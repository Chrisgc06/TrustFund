using TrustFund.Services.Contracts;
using TrustFund.Services.Dtos.Loan;
using TrustFund.Services.Exceptions; // Importa tu namespace de excepciones
using TrustFund.Domain.Entities;
using TrustFund.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Para Validacion de DTOs
using System.Linq;

namespace TrustFund.Services.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LoanService(ILoanRepository loanRepository, IUserRepository userRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
        {
            // Validaciones de DTO
            var validationContext = new ValidationContext(createLoanDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(createLoanDto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage)
                                              .Where(e => e != null) // **CORRECCIÓN:** Filtra nulos
                                              .ToList();
                throw new TrustFund.Services.Exceptions.ValidationException(errors);
            }

            // Validaciones de Negocio
            var lender = await _userRepository.GetByIdAsync(createLoanDto.LenderId);
            if (lender == null)
            {
                throw new NotFoundException(nameof(User), createLoanDto.LenderId);
            }

            var borrower = await _userRepository.GetByIdAsync(createLoanDto.BorrowerId);
            if (borrower == null)
            {
                throw new NotFoundException(nameof(User), createLoanDto.BorrowerId);
            }

            if (createLoanDto.LenderId == createLoanDto.BorrowerId)
            {
                throw new TrustFund.Services.Exceptions.ValidationException("Lender and Borrower cannot be the same user for a loan.");
            }

            if (createLoanDto.DueDate < DateTime.Today)
            {
                throw new TrustFund.Services.Exceptions.ValidationException("Loan due date cannot be in the past.");
            }

            var loan = _mapper.Map<Loan>(createLoanDto);
            loan.StartDate = DateTime.Today;
            loan.OutstandingAmount = createLoanDto.Amount;
            loan.Status = LoanStatus.Pending;

            await _loanRepository.AddAsync(loan);

            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LenderName = lender.Name;
            loanDto.BorrowerName = borrower.Name;
            return loanDto;
        }

        public async Task<LoanDto?> GetLoanByIdAsync(Guid id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), id);
            }
            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LenderName = (await _userRepository.GetByIdAsync(loan.LenderId))?.Name;
            loanDto.BorrowerName = (await _userRepository.GetByIdAsync(loan.BorrowerId))?.Name;
            return loanDto;
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId)
        {
            var loans = await _loanRepository.GetLoansByUserIdAsync(userId);
            var loanDtos = _mapper.Map<List<LoanDto>>(loans);

            foreach (var loanDto in loanDtos)
            {
                loanDto.LenderName = (await _userRepository.GetByIdAsync(loanDto.LenderId))?.Name;
                loanDto.BorrowerName = (await _userRepository.GetByIdAsync(loanDto.BorrowerId))?.Name;
            }
            return loanDtos;
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _loanRepository.GetOverdueLoansAsync();
            var loanDtos = _mapper.Map<List<LoanDto>>(loans);

            foreach (var loanDto in loanDtos)
            {
                loanDto.LenderName = (await _userRepository.GetByIdAsync(loanDto.LenderId))?.Name;
                loanDto.BorrowerName = (await _userRepository.GetByIdAsync(loanDto.BorrowerId))?.Name;
            }
            return loanDtos;
        }

        public async Task UpdateLoanAsync(Guid id, UpdateLoanDto updateLoanDto)
        {
            // Validaciones de DTO
            var validationContext = new ValidationContext(updateLoanDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(updateLoanDto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage)
                                              .Where(e => e != null) // **CORRECCIÓN:** Filtra nulos
                                              .ToList();
                throw new TrustFund.Services.Exceptions.ValidationException(errors);
            }

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), id);
            }

            // Validaciones de Negocio para la actualización
            if (updateLoanDto.DueDate.HasValue && updateLoanDto.DueDate.Value < DateTime.Today && loan.Status == LoanStatus.Pending)
            {
                throw new TrustFund.Services.Exceptions.ValidationException("Cannot set due date in the past for a pending loan.");
            }

            if (updateLoanDto.Status.HasValue)
            {
                if (updateLoanDto.Status.Value == LoanStatus.Paid && loan.OutstandingAmount > 0)
                {
                    throw new TrustFund.Services.Exceptions.ValidationException("Cannot mark loan as paid if outstanding amount is greater than zero. Process payments first.");
                }
            }

            _mapper.Map(updateLoanDto, loan); // Mapea los valores del DTO a la entidad existente

            await _loanRepository.UpdateAsync(loan);
        }

        public async Task DeleteLoanAsync(Guid id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), id);
            }

            // Validación de negocio: No borrar préstamo si tiene pagos o está pendiente
            if (loan.Status == LoanStatus.Pending || loan.Status == LoanStatus.Overdue)
            {
                throw new TrustFund.Services.Exceptions.ValidationException("Cannot delete a loan that is pending or overdue. It must be paid or explicitly canceled first.");
            }

            await _loanRepository.DeleteAsync(id);
        }

        public async Task<decimal> CalculateInterestAsync(Guid loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), loanId);
            }

            // Suponemos interés simple acumulado desde la fecha de inicio hasta hoy
            TimeSpan duration = DateTime.Today - loan.StartDate;
            int days = (int)duration.TotalDays;

            if (days < 0) days = 0; // No se acumula interés antes de la fecha de inicio

            decimal interest = loan.Amount * loan.InterestRate * (days / 365m); // Usar 365m para asegurar cálculo decimal

            return interest;
        }
    }
}
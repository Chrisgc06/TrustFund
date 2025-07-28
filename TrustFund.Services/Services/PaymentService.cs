using TrustFund.Services.Contracts;
using TrustFund.Services.Dtos.Payment;
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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, ILoanRepository loanRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            // Validaciones de DTO
            var validationContext = new ValidationContext(createPaymentDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(createPaymentDto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage)
                                              .Where(e => e != null) // **CORRECCIÓN:** Filtra nulos
                                              .ToList();
                throw new TrustFund.Services.Exceptions.ValidationException(errors);
            }

            // Validaciones de Negocio
            var loan = await _loanRepository.GetByIdAsync(createPaymentDto.LoanId);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), createPaymentDto.LoanId);
            }

            if (loan.Status == LoanStatus.Paid || loan.Status == LoanStatus.Canceled)
            {
                throw new TrustFund.Services.Exceptions.ValidationException($"Cannot add payment to a loan that is already '{loan.Status.ToString().ToLower()}'.");
            }

            if (createPaymentDto.AmountPaid <= 0)
            {
                throw new TrustFund.Services.Exceptions.ValidationException("Payment amount must be greater than zero.");
            }

            if (createPaymentDto.AmountPaid > loan.OutstandingAmount)
            {
                throw new TrustFund.Services.Exceptions.ValidationException($"Payment amount (${createPaymentDto.AmountPaid}) exceeds the remaining outstanding amount (${loan.OutstandingAmount}).");
            }

            var payment = _mapper.Map<Payment>(createPaymentDto);
            payment.PaymentDate = DateTime.Now; // La fecha del pago es la actual

            await _paymentRepository.AddAsync(payment);

            // Actualizar el monto pendiente del préstamo y su estado si aplica
            loan.OutstandingAmount -= payment.AmountPaid;

            if (loan.OutstandingAmount <= 0)
            {
                loan.OutstandingAmount = 0; // Asegurarse de que no sea negativo
                loan.Status = LoanStatus.Paid;
            }
            else if (loan.DueDate < DateTime.Today && loan.Status == LoanStatus.Pending)
            {
                // Si el pago no salda la deuda y ya se venció, el estado sigue siendo Overdue si ya lo era, o se actualiza
                loan.Status = LoanStatus.Overdue;
            }

            await _loanRepository.UpdateAsync(loan); // Guardar los cambios en el préstamo

            var paymentDto = _mapper.Map<PaymentDto>(payment);
            paymentDto.LoanDescription = $"Loan (ID: {loan.Id}) from {loan.LenderId} to {loan.BorrowerId}"; // Simplificado
            return paymentDto;
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new NotFoundException(nameof(Payment), id);
            }
            var paymentDto = _mapper.Map<PaymentDto>(payment);
            var loan = await _loanRepository.GetByIdAsync(payment.LoanId);
            paymentDto.LoanDescription = loan != null ? $"Loan (ID: {loan.Id}) from {loan.LenderId} to {loan.BorrowerId}" : "Loan not found";
            return paymentDto;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByLoanIdAsync(Guid loanId)
        {
            var payments = await _paymentRepository.GetPaymentsByLoanIdAsync(loanId);
            var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);

            var loan = await _loanRepository.GetByIdAsync(loanId);
            string loanDescription = loan != null ? $"Loan (ID: {loan.Id}) from {loan.LenderId} to {loan.BorrowerId}" : "Loan not found";

            foreach (var paymentDto in paymentDtos)
            {
                paymentDto.LoanDescription = loanDescription;
            }
            return paymentDtos;
        }
    }
}
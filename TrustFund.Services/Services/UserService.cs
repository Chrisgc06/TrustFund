using TrustFund.Services.Contracts;
using TrustFund.Services.Dtos.User;
using TrustFund.Services.Exceptions; // Importa tu namespace de excepciones
using TrustFund.Domain.Entities;
using TrustFund.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Para Validacion de DTOs
using System.Linq; // Para Where, Select, etc.

namespace TrustFund.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        // private readonly ILoanRepository _loanRepository; // Podría ser necesario para validar si se puede borrar un usuario

        public UserService(IUserRepository userRepository, IMapper mapper /*, ILoanRepository loanRepository*/)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            // _loanRepository = loanRepository;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validaciones de DTO usando Data Annotations
            var validationContext = new ValidationContext(createUserDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(createUserDto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage)
                                              .Where(e => e != null) // **CORRECCIÓN:** Filtra nulos
                                              .ToList();
                throw new TrustFund.Services.Exceptions.ValidationException(errors);
            }

            // Validaciones de Negocio
            if (await _userRepository.GetByEmailAsync(createUserDto.Email) != null)
            {
                throw new TrustFund.Services.Exceptions.ValidationException($"A user with email '{createUserDto.Email}' already exists.");
            }

            var user = _mapper.Map<User>(createUserDto);
            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), id);
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            // Validaciones de DTO (parciales)
            var validationContext = new ValidationContext(updateUserDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(updateUserDto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage)
                                              .Where(e => e != null) // **CORRECCIÓN:** Filtra nulos
                                              .ToList();
                throw new TrustFund.Services.Exceptions.ValidationException(errors);
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), id);
            }

            // Validar si el nuevo email ya está en uso por otro usuario
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    throw new TrustFund.Services.Exceptions.ValidationException($"Another user with email '{updateUserDto.Email}' already exists.");
                }
            }

            _mapper.Map(updateUserDto, user); // AutoMapper maneja el mapeo de actualizaciones parciales

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), id);
            }

            // Lógica de validación de negocio: No borrar usuario si tiene préstamos activos
            // if (await _loanRepository.HasActiveLoansForUserAsync(id)) // Requiere inyectar ILoanRepository
            // {
            //     throw new TrustFund.Services.Exceptions.ValidationException("Cannot delete user with active loans.");
            // }

            await _userRepository.DeleteAsync(id);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), email);
            }
            return _mapper.Map<UserDto>(user);
        }
    }
}
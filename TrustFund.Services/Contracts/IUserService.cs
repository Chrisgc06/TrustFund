using TrustFund.Services.Dtos.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrustFund.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> GetUserByIdAsync(Guid id); 
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(Guid id);
        Task<UserDto?> GetUserByEmailAsync(string email); 
    }
}
using System;

namespace TrustFund.Services.Dtos.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } 
        public required string Email { get; set; } 
        public required string PhoneNumber { get; set; } 
    }
}
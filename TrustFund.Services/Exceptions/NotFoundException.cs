using System;

namespace TrustFund.Services.Exceptions
{
    public class NotFoundException : ServiceException 
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.") { }
    }
}
using System;
using System.Collections.Generic;
using System.Linq; 

namespace TrustFund.Services.Exceptions
{
    public class ValidationException : ServiceException 
    {
        public List<string> Errors { get; }

        public ValidationException() : base("One or more validation errors occurred.")
        {
            Errors = new List<string>();
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(IEnumerable<string> errors) : base("One or more validation errors occurred.")
        {
            Errors = errors.ToList(); 
        }
    }
}
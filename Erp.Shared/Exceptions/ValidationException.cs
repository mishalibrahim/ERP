using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Shared.Exceptions
{
    public class ValidationException :Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}

using System;

namespace Restaurant_WebAPI.Exceptions
{
    public class AccountRequestException : Exception
    {
        public AccountRequestException() : base()
        {
        }

        public AccountRequestException(string message) : base(message)
        {
        }

        public AccountRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

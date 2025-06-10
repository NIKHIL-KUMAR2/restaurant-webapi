using System;

namespace Restaurant_WebAPI.Util
{
    public class OrderCreationException:Exception
    {
        public OrderCreationException()
            : base("An error occurred while creating the order.")
        {
        }

        public OrderCreationException(string message)
            : base(message)
        {
        }

        public OrderCreationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

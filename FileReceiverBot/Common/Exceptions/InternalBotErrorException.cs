using System;

namespace FileReceiverBot.Common.Exceptions
{
    public class InternalBotErrorException : Exception
    {
        public InternalBotErrorException() : base()
        {
        }

        public InternalBotErrorException(string message) : base(message)
        {
        }

        public InternalBotErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

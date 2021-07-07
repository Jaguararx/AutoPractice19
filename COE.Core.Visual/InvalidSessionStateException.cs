using System;

namespace COE.Core.Visual
{
    public class InvalidSessionStateException : Exception
    {
        public InvalidSessionStateException()
        {
        }

        public InvalidSessionStateException(string message)
            : base(message)
        {
        }

        public InvalidSessionStateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
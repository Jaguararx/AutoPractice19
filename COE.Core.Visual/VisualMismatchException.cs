using System;

namespace COE.Core.Visual
{
    public class VisualMismatchException : Exception
    {
        public VisualMismatchException()
        {
        }

        public VisualMismatchException(string message)
            : base(message)
        {
        }

        public VisualMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
using System;

namespace COE.Core.Visual
{
    public class ImageSizeMismatchException : Exception
    {
        public ImageSizeMismatchException()
        {
        }

        public ImageSizeMismatchException(string message)
            : base(message)
        {
        }

        public ImageSizeMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
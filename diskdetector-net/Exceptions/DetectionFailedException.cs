using System;

namespace DiskDetector.Exceptions
{
    public class DetectionFailedException : Exception
    {
        public DetectionFailedException()
        {
        }

        public DetectionFailedException(string message)
            : base(message)
        {
        }

        public DetectionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
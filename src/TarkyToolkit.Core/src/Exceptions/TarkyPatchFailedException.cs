using System;

namespace TarkyToolkit.Core.Exceptions
{
    public class TarkyPatchFailedException : Exception
    {
        public TarkyPatchFailedException(bool isFatal, Exception innerException)
        {
            IsFatal = isFatal;
            InnerException = innerException;
        }

        public bool IsFatal { get; }

        public new Exception InnerException { get; }
    }
}

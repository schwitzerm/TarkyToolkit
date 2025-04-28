namespace TarkyToolkit.Core.Exceptions;

public class TarkyPatchFailedException(bool isFatal, Exception innerException) : Exception
{
    public bool IsFatal { get; } = isFatal;

    public new Exception InnerException { get; } = innerException;
}

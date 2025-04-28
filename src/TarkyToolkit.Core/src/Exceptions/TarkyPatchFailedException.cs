namespace TarkyToolkit.Core.Exceptions;

public class TarkyPatchFailedException(bool isFatal) : Exception
{
    public bool IsFatal { get; } = isFatal;
}

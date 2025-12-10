namespace FS.Core.Exceptions;

public class DomainException(string issue, string message, string? field = null) : Exception(message)
{
    public string Issue { get; } = issue;
    public string? Field { get; } = field;
}
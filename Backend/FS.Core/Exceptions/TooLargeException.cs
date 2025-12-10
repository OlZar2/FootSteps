using FS.Contracts.Error;

namespace FS.Core.Exceptions;

public class TooLargeException(string message, string? field = null)
    : DomainException(IssueCodes.TooLarge, message, field)
{
    
}
using FS.Contracts.Error;

namespace FS.Core.Exceptions;

public class TooSmallException(string message, string? field = null)
    : DomainException(IssueCodes.TooSmall, message, field)
{
    
}
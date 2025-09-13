namespace FS.Core.Exceptions;

public class NotEnoughRightsException(string issue, string message, string? field = null) 
    : DomainException(issue, message, field)
{
    
}
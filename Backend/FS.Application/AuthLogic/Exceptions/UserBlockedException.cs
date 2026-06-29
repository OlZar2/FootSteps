namespace FS.Application.AuthLogic.Exceptions;

public class UserBlockedException(string reason) : Exception
{
    public string Reason { get; } = reason;
}

namespace FS.Application.Shared.Exceptions;

public class NotFoundException : Exception
{
    public string? EntityName { get; }
    public object? Key { get; }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} с ключом '{key}' не найден.")
    {
        EntityName = entityName;
        Key = key;
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
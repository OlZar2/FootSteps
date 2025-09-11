namespace FS.Application.Exceptions;

public class NotFoundException(string entityName, object key) : Exception($"{entityName} с ключом '{key}' не найден.")
{
    public string EntityName { get; } = entityName;
    public object Key { get; } = key;
}
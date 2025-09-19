using System.ComponentModel.DataAnnotations;
using FS.Core.ValueObjects.Abstract;

namespace FS.Core.ValueObjects;

public class FullName : ValueObject
{
    public string FirstName { get; }
    public string? SecondName { get; }
    public string? Patronymic { get; }

    private FullName(string firstName, string? secondName, string? patronymic)
    {
        FirstName = firstName;
        SecondName = secondName;
        Patronymic = patronymic;
    }

    public static FullName Create(string firstName, string? secondName, string? patronymic)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ValidationException("Имя не может быть пустым");
        }

        return new FullName(firstName, secondName, patronymic);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return SecondName;
        yield return Patronymic;
    }
}
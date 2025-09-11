using FS.Contracts.Error;
using FS.Core.Exceptions;

namespace FS.Core.ValueObjects;

public class Place
{
    public string Value { get; }

    private Place(string value) => Value = value;

    public static Place Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(IssueCodes.Required, "Место не может быть пустым", "Place");
        return new Place(value);
    }
}
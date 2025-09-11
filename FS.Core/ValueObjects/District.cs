using FS.Contracts.Error;
using FS.Core.Exceptions;

namespace FS.Core.ValueObjects;

public class District
{
    public string Value { get; }

    private District(string value) => Value = value;

    public static District Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(IssueCodes.Required, "Район не может быть пустым", nameof(District));
        return new District(value);
    }
}
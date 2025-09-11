using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FS.Core.ValueObjects.Abstract;

namespace FS.Core.ValueObjects;

public partial class Email : ValueObject
{
    public string Value { get; }
    
    private static readonly Regex EmailRegex = MyRegex();

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email не может быть пустым");

        if (!EmailRegex.IsMatch(email))
            throw new ValidationException("Неверный формат email");

        return new Email(email);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "ru-RU")]
    private static partial Regex MyRegex();
}
using System.Text.RegularExpressions;
using FS.Core.ValueObjects.Abstract;

namespace FS.Core.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }
    
    private static readonly Regex PhoneRegex = new Regex(
        @"^(?:\+7|8)\d{10}$",
        RegexOptions.Compiled);

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneString)
    {
        if (string.IsNullOrWhiteSpace(phoneString))
            throw new ArgumentException("Телефон не может быть пустым");

        if (!PhoneRegex.IsMatch(phoneString))
            throw new ArgumentException("Неверный формат телефона");

        return new PhoneNumber(phoneString);
    }

    public override string ToString() => Value;

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
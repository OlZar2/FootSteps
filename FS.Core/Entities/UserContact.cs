using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Exceptions;

namespace FS.Core.Entities;

public class UserContact
{
    public Guid Id { get; private set; }
    
    public ContactType Type { get; private set; }
    
    public string Url  { get; private set; }
    
    private UserContact(ContactType type, string url)
    {
        Url = url;
        Type = type;
    }

    public static UserContact Create(ContactType type, string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new DomainException
            (IssueCodes.Required, "URL is required.", nameof(url));
        
        return new UserContact(type, url);
    }

    internal void Update(ContactType type, string url)
    {
        SetType(type);
        SetUrl(url);
    }

    private void SetType(ContactType type)
    {
        Type = type;
    }

    private void SetUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new DomainException
            (IssueCodes.Required, "URL is required.", nameof(url));
        
        Url = url;
    }
}
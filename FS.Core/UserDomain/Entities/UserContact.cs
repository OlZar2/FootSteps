using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.Shared.Abstractions;
using FS.Core.UserDomain.Enums;

namespace FS.Core.UserDomain.Entities;

public class UserContact : Entity
{
    public ContactType Type { get; private set; }
    
    public string Url  { get; private set; }
    
    private UserContact(ContactType type, string url, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Url = url;
        Type = type;
    }

    public static UserContact Create(ContactType type, string url, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new DomainException
            (IssueCodes.Required, "URL is required.", nameof(url));
        
        return new UserContact(type, url, id);
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
    
    // EF
    private UserContact(){ }
}
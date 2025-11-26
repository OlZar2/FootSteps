using FS.Core.Entities;
using FS.Core.Enums;

namespace FS.Application.DTOs.AuthDTOs;

public record MeContactData
{
    public required ContactType ContactType { get; set; }
    public required string Url { get; set; }
    
    public static MeContactData From(UserContact userContact)
    {
        return new MeContactData()
        {
            ContactType = userContact.Type,
            Url = userContact.Url,
        };
    }
}
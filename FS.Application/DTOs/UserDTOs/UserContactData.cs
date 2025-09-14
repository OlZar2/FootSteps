using FS.Core.Entities;
using FS.Core.Enums;

namespace FS.Application.DTOs.UserDTOs;

public record UserContactData
{
    public required ContactType ContactType { get; set; }
    public required string Url { get; set; }

    public static UserContactData From(UserContact userContact)
    {
        return new UserContactData()
        {
            ContactType = userContact.Type,
            Url = userContact.Url,
        };
    }
}
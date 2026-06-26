using FS.Core.UserDomain.Entities;
using FS.Core.UserDomain.Enums;

namespace FS.Application.AuthLogic.DTOs;

public record ContactData
{
    public required ContactType ContactType { get; set; }
    public required string Url { get; set; }
}
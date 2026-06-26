using FS.Application.AuthLogic.DTOs;
using FS.Core.UserDomain.Entities;

namespace FS.Application.MissingPetLogic.DTOs;

public record SpottedUserDto(Guid Id, string FirstName, string? SecondName, ContactData[] Contacts) { }
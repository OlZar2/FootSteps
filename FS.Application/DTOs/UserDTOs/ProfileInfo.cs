using FS.Core.Entities;

namespace FS.Application.DTOs.UserDTOs;

public record ProfileInfo
{
    public required string FirstName { get; init; }
    public required string? SecondName { get; init; }
    public required string? Patronymic { get; init; }
    
    public required string? Description { get; init; }
    
    public required UserContactData[]  UserContacts { get; init; }
    
    public required FindAnnouncement[]  FindAnnouncements { get; init; }
    public required MissingAnnouncement[]  MissingAnnouncements { get; init; }
}
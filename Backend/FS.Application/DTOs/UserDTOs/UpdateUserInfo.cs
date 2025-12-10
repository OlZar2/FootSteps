namespace FS.Application.DTOs.UserDTOs;

public record UpdateUserInfo
{
    public required Guid UserId { get; set; }
    
    public required string? FirstName { get; init; }
    
    public required string? SecondName { get; init; }
    
    public required string? Patronymic { get; init; }
    
    public required string? Description { get; init; }
    
    public required UserContactData[]? UserContacts { get; init; }
}
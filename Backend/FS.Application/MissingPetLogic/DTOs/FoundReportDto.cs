namespace FS.Application.MissingPetLogic.DTOs;

public record FoundReportDto(Guid Id,
    SpottedUserDto SpottedUser,
    DateTime CreatedAt,
    string[] ImagesPath);
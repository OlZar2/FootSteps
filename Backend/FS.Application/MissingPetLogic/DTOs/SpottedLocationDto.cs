using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.DTOs;

public record SpottedLocationDto(
    Guid Id,
    SpottedUserDto SpottedUser,
    DateTime CreatedAt,
    CoordinatesDto Location,
    string[] ImagesPath) { }
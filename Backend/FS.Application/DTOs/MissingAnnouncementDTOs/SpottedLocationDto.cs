using FS.Application.DTOs.Shared;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record SpottedLocationDto(Guid Id, SpottedUserDto SpottedUser, DateTime CreatedAt, CoordinatesDto Location) { }
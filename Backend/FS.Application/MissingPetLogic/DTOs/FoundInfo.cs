using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.DTOs;

public record FoundInfo(Guid FoundUserId, Guid AnnouncementId, FileData[] Images) { }
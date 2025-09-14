using FS.Core.Enums;

namespace FS.Application.DTOs.FindAnnouncementDTOs;

public class DeleteFindAnnouncementData
{
    public required Guid DeleterId { get; set; }
    
    public required Guid AnnouncementId { get; set; }
    
    public required FindAnnouncementDeleteReason  DeleteReason { get; set; }
}
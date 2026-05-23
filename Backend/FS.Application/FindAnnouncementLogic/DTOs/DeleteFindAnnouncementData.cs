using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.FindAnnouncementLogic.DTOs;

public class DeleteFindAnnouncementData
{
    public required Guid DeleterId { get; set; }
    
    public required Guid AnnouncementId { get; set; }
    
    public required FindAnnouncementDeleteReason  DeleteReason { get; set; }
}
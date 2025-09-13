using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public class DeleteMissingAnnouncementData
{
    public required Guid DeleterId { get; set; }
    
    public required Guid AnnouncementId { get; set; }
    
    public required MissingAnnouncementDeleteReasons  DeleteReason { get; set; }
}
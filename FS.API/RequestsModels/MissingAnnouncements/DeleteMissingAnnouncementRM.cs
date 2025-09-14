using FS.Core.Enums;

namespace FS.API.RequestsModels.MissingAnnouncements;

public class DeleteMissingAnnouncementRM
{
    public MissingAnnouncementDeleteReason? DeleteReason { get; set; }
}
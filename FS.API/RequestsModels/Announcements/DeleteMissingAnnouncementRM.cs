using FS.Core.Enums;

namespace FS.API.RequestsModels.Announcements;

public class DeleteMissingAnnouncementRM
{
    public MissingAnnouncementDeleteReasons? DeleteReason { get; set; }
}
using System.ComponentModel;

namespace FS.API.Controllers.MissingAnnouncement.RequestModels;

public class CancelMissingAnnouncementRM
{
    [Description("Причина удаления. Обязательно. Issue REQUIRED. 0 = Питомец нашелся, 1 = Другое. " +
                 "Если неверное значение Issue INVALID_VALUE")]
    public required int DeleteReason { get; set; }
}
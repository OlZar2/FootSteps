using System.ComponentModel;
using FS.Core.Enums;

namespace FS.API.RequestsModels.MissingAnnouncements;

public class CancelMissingAnnouncementRM
{
    [Description("Причина удаления. Обязательно. Issue REQUIRED. 0 = Питомец нашелся, 1 = Другое. " +
                 "Если неверное значение Issue INVALID_VALUE")]
    public required int DeleteReason { get; set; }
}
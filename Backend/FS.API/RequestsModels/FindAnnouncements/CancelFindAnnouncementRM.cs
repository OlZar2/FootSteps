using System.ComponentModel;

namespace FS.API.RequestsModels.FindAnnouncements;

public class CancelFindAnnouncementRM
{
    [Description("Причина удаления. Обязательно. Issue REQUIRED. 0 = Владелец нашелся, 1 = Оставил себе. " +
                 "2 = Отдал другому, 3 = Никто не откликается, 4 = Дргуое. Если неверное значение Issue INVALID_VALUE")]
    public int CancelReason { get; set; }
}
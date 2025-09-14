using System.ComponentModel;
using FS.Core.Enums;

namespace FS.API.RequestsModels.Auth;

public record UserContactRM
{
    [Description("Тип контакта. 0 = VK, 1 = Telegram, 2 = Whatsapp. Если неверное значение issue INVALID_VALUE.")]
    public int ContactType { get; set; }
    [Description("Ссылка. Обязательна. Issue REQUIRED.")]
    public required string Url { get; set; }
}
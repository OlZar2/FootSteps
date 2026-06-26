using System.ComponentModel;

namespace FS.API.Controllers.MissingAnnouncement.RequestModels;

public class ReportFoundRM
{
    [Description("Картинки. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE" +
                 "Максимум 5 картинок. Если больше то issue TOO_MANY")]
    public required IFormFile[] Images  { get; init; }
}
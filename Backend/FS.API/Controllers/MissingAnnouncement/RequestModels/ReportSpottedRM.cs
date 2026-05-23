using System.ComponentModel;
using FS.Application.Shared.DTOs;

namespace FS.API.Controllers.MissingAnnouncement.RequestModels;

public class ReportSpottedRM
{
    [Description("Картинки. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE" +
                 "Максимум 5 картинок. Если больше то issue TOO_MANY")]
    public required IFormFile[] Images  { get; init; }
    
    [Description("Точка, где питомец был замечен")]
    public required CoordinatesDto Coordinates { get; init; }
}
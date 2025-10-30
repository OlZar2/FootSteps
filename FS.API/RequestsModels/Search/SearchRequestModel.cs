using System.ComponentModel;

namespace FS.API.RequestsModels.Search;

public class SearchRequestModel
{
    [Description("Картинки. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE")]
    public required IFormFile Image { get; set; }
}
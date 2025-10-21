using System.ComponentModel;
using FS.Application.DTOs.Shared;

namespace FS.API.RequestsModels.MissingAnnouncements;

public class CreateMissingAnnouncementRM
{
    [Description("Координаты. Обязательно, issue REQUIRED.")]
    public required Coordinates Location { get; init; }
    
    [Description("Картинки. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE")]
    public required IFormFile[] Images  { get; init; }
    
    [Description("Тип питомца. Обязательно, issue REQUIRED. Если неверное значение " +
                 "issue INVALID_VALUE. Значения: 0 = Кот, 1 = Собака, 2 = Другое")]
    public required int? PetType { get; init; }
    [Description("Пол питомца. Обязательно, issue REQUIRED. Если неверное значение " +
                 "issue INVALID_VALUE. Значения: 0 = М, 1 = Ж, 2 = Неизвестно")]
    public required int? Gender { get; init; }
    [Description("Окраска питомца. Если больше 50 символов issue TOO_LONG")]
    public string? Color { get; init; }
    [Description("Порода питомца. Если больше 50 символов issue TOO_LONG")]
    public string? Breed { get; init; }
    [Description("Кличка питомца. Обязательно, issue REQUIRED. Если больше 50 символов issue TOO_LONG")]
    public required string PetName { get; init; }
    
    [Description("Время пропажи. Обязательно, issue REQUIRED.")]
    public required DateTime? EventDate { get; init; }
    
    [Description("Описание объявления")]
    public string? Description { get; init; }
}
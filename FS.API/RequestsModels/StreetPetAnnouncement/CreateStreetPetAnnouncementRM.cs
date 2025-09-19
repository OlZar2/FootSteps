using System.ComponentModel;
using FS.Application.DTOs.Shared;

namespace FS.API.RequestsModels.StreetPetAnnouncement;

public class CreateStreetPetAnnouncementRM
{
    [Description("Полное место(улица, район, дом?). Обязательно, issue REQUIRED.")]
    public required string FullPlace { get; init; }
    
    [Description("Только район. Обязательно, issue REQUIRED.")]
    public required string District { get; init; }
    
    [Description("Картинки. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE")]
    public required IFormFile[] Images  { get; init; }
    
    [Description("Тип питомца. Обязательно, issue REQUIRED. Если неверное значение " +
                 "issue INVALID_VALUE. Значения: 0 = Кот, 1 = Собака, 2 = Другое")]
    public required int? PetType { get; init; }
    
    [Description("Координаты. Обязательно, issue REQUIRED.")]
    public required Coordinates Location { get; init; }
    
    [Description("Время пропажи. Обязательно, issue REQUIRED.")]
    public required DateTime? EventDate { get; init; }
    
    [Description("Описание места.")]
    public string? PlaceDescription { get; init; }
}
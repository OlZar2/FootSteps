using System.ComponentModel;
using FS.Application.DTOs.Shared;

namespace FS.API.RequestsModels.StreetPetAnnouncement;

public class CreateStreetPetAnnouncementRM
{
    [Description("Id Картинок. Обязательно, issue REQUIRED. " +
                 "Максимум 5 id. Если больше то issue TOO_MANY")]
    public required Guid[] ImageIds  { get; init; }
    
    [Description("Тип питомца. Обязательно, issue REQUIRED. Если неверное значение " +
                 "issue INVALID_VALUE. Значения: 0 = Кот, 1 = Собака, 2 = Другое")]
    public required int? PetType { get; init; }
    
    [Description("Координаты. Обязательно, issue REQUIRED.")]
    public required CoordinatesDto Location { get; init; }
    
    [Description("Время пропажи. Обязательно, issue REQUIRED.")]
    public required DateTime? EventDate { get; init; }
    
    [Description("Описание места.")]
    public string? PlaceDescription { get; init; }
}
using System.ComponentModel;
using FS.Application.DTOs.Shared;

namespace FS.API.RequestsModels.FindAnnouncements;

public class CreateFindAnnouncementRM
{
    [Description("Координаты. Обязательно, issue REQUIRED.")]
    public required CoordinatesDto Location { get; init; }
    
    [Description("Id Картинок. Обязательно, issue REQUIRED. " +
                 "Максимум 5 id. Если больше то issue TOO_MANY")]
    public required Guid[] ImageIds  { get; init; }
    
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
    
    [Description("Время пропажи. Обязательно, issue REQUIRED.")]
    public required DateTime? EventDate { get; init; }
    
    [Description("Описание объявления")]
    public string? Description { get; init; }
}
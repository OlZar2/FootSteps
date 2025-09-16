using System.ComponentModel;
using FS.Core.Enums;

namespace FS.Application.DTOs.Shared;

public record AnnouncementFilter
{
    [Description("Фильтр по району")]
    public string? District { get; init; }
    [Description("Дата после которой показываются объявления")]
    public DateTime? From { get; init; }

    [Description("Тип питомца 0 = Кот, 1 = Собака, 2 = Другое")]
    public PetType? Type { get; init; }
    [Description("Пол питомца  0 = М, 1 = Ж, 2 = Неизвестно")]
    public Gender? Gender { get; init; }
}
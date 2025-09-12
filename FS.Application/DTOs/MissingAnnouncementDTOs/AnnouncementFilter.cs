using System.ComponentModel;
using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record AnnouncementFilter
{
    [Description("Фильтр по району")]
    public string? District { get; init; }
    [Description("Дата после которой показываются объявления")]
    public DateTime? From { get; init; }

    [Description("Тип питомца")]
    public PetType? Type { get; init; }
    [Description("Пол питомца")]
    public Gender? Gender { get; init; }
}
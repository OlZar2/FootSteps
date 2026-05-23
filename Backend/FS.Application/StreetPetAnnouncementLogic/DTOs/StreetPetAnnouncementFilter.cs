using System.ComponentModel;
using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.StreetPetAnnouncementLogic.DTOs;

//TODO: сделать фильтры наслдуемыми
public class StreetPetAnnouncementFilter
{
    [Description("Фильтр по району")]
    public string? District { get; init; }
    
    [Description("Дата после которой показываются объявления")]
    public DateTime? From { get; init; }

    [Description("Тип питомца 0 = Кот, 1 = Собака, 2 = Другое")]
    public PetType? Type { get; init; }
    
    [Description("Радиус поиска объявлений. Максимум 40 км.")]
    public int? SearchRadius { get; init; }
    
    [Description("Точка поиска центра объявлений")]
    public CoordinatesDto? SearchCenter { get; init; }
}
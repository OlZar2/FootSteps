using System.Linq.Expressions;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.Shared.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.AnimalAnnouncementBC.Specifications;

//TODO: сделать спецификации наследуемыми
public class StreetPetAnnouncementFeedSpecification
{
    public StreetPetAnnouncementFeedSpecification(
        string? district = null,
        DateTime? from = null,
        PetType? petType = null,
        Point? searchCenter = null,
        int? searchRadius =  null,
        Sorting? sorting = null,
        params Expression<Func<StreetPetAnnouncement, object>>[] includes)
    {
        Criteria = ma =>
            (string.IsNullOrEmpty(district) || ma.District == district) &&
            (from == null || ma.CreatedAt >= from) &&
            (petType == null || ma.PetType == petType) &&
            (searchCenter == null || searchRadius == null ||
             ma.Location.IsWithinDistance(searchCenter, searchRadius.Value * 1000));

        Sorting = sorting;
        Includes = includes.ToList();
    }

    public Expression<Func<StreetPetAnnouncement, bool>> Criteria { get; }
    
    public IReadOnlyList<Expression<Func<StreetPetAnnouncement, object>>> Includes { get; }
    
    public Sorting? Sorting { get; }
}
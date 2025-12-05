using System.Linq.Expressions;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC.Specifications;

public class StreetPetAnnouncementFeedSpecification
{
    public StreetPetAnnouncementFeedSpecification(
        string? district = null, DateTime? from = null, PetType? petType = null,
        Sorting? sorting = null, params Expression<Func<StreetPetAnnouncement, object>>[] includes)
    {
        Criteria = ma =>
            (string.IsNullOrEmpty(district) || ma.District == district) &&
            (from == null || ma.CreatedAt >= from) &&
            (petType == null || ma.PetType == petType);

        Sorting = sorting;
        Includes = includes?.ToList() ?? [];
    }

    public Expression<Func<StreetPetAnnouncement, bool>> Criteria { get; }
    
    public IReadOnlyList<Expression<Func<StreetPetAnnouncement, object>>> Includes { get; }
    
    public Sorting? Sorting { get; }
}
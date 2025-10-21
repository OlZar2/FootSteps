using System.Linq.Expressions;
using FS.Core.Entities;
using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Specifications;

public class PetAnnouncementFeedSpecification<T> where T : PetAnnouncement
{
    public PetAnnouncementFeedSpecification(
        string? district = null, DateTime? from = null, PetType? petType = null, Gender? gender = null,
        Sorting? sorting = null, params Expression<Func<T, object>>[] includes)
    {
        Criteria = ma =>
            (string.IsNullOrEmpty(district) || ma.District == district) &&
            (from == null || ma.CreatedAt >= from) &&
            (petType == null || ma.PetType == petType) &&
            (gender == null || ma.Gender == gender);

        Sorting = sorting;
        Includes = includes.ToList();
    }

    public Expression<Func<T, bool>> Criteria { get; }
    
    public IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
    
    public Sorting? Sorting { get; }
}
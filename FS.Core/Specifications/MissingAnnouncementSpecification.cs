using System.Linq.Expressions;
using FS.Core.Entities;
using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Specifications;

public class MissingAnnouncementSpecification
{
    public MissingAnnouncementSpecification(
        string? district = null, DateTime? from = null, PetType? petType = null, Gender? gender = null,
        Sorting? sorting = null, params Expression<Func<MissingAnnouncement, object>>[] includes)
    {
        Criteria = ma =>
            (string.IsNullOrEmpty(district) || ma.District.Value == district) &&
            (from == null || ma.CreatedAt >= from) &&
            (petType == null || ma.PetType == petType) &&
            (gender == null || ma.Gender == gender);

        Sorting = sorting;
        Includes = includes?.ToList() ?? new List<Expression<Func<MissingAnnouncement, object>>>();
    }

    public Expression<Func<MissingAnnouncement, bool>> Criteria { get; }
    
    public IReadOnlyList<Expression<Func<MissingAnnouncement, object>>> Includes { get; }
    
    public Sorting? Sorting { get; }
}
using FluentValidation;
using FS.Application.AnnouncementLogic.DTOs;
using FS.Contracts.Error;

namespace FS.API.Controllers.AnimalAnnouncement.RequestModels.Validators;

public sealed class AdminAnimalAnnouncementListQueryValidator : AbstractValidator<AdminAnimalAnnouncementListQuery>
{
    public AdminAnimalAnnouncementListQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(AdminAnimalAnnouncementListQuery.MinPageSize)
            .WithMessage($"Page size must be at least {AdminAnimalAnnouncementListQuery.MinPageSize}.")
            .WithErrorCode(IssueCodes.TooSmall)
            .LessThanOrEqualTo(AdminAnimalAnnouncementListQuery.MaxPageSize)
            .WithMessage($"Page size cannot be more than {AdminAnimalAnnouncementListQuery.MaxPageSize}.")
            .WithErrorCode(IssueCodes.TooLarge);

        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("SortBy has invalid value.")
            .WithErrorCode(IssueCodes.InvalidValue);

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .WithMessage("SortDirection has invalid value.")
            .WithErrorCode(IssueCodes.InvalidValue);

        RuleFor(x => x.Cursor)
            .Must(cursor => AdminAnimalAnnouncementCursor.TryDecode(cursor, out _))
            .WithMessage("Cursor has invalid format.")
            .WithErrorCode(IssueCodes.InvalidFormat)
            .When(x => !string.IsNullOrWhiteSpace(x.Cursor));

        RuleFor(x => x.CreatedAtFrom)
            .LessThanOrEqualTo(x => x.CreatedAtTo)
            .WithMessage("CreatedAtFrom cannot be later than CreatedAtTo.")
            .WithErrorCode(IssueCodes.InvalidDate)
            .When(x => x.CreatedAtFrom.HasValue && x.CreatedAtTo.HasValue);

        RuleFor(x => x.ReportsCountFrom)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ReportsCountFrom cannot be negative.")
            .WithErrorCode(IssueCodes.TooSmall)
            .When(x => x.ReportsCountFrom.HasValue);

        RuleFor(x => x.ReportsCountTo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ReportsCountTo cannot be negative.")
            .WithErrorCode(IssueCodes.TooSmall)
            .When(x => x.ReportsCountTo.HasValue);

        RuleFor(x => x.ReportsCountFrom)
            .LessThanOrEqualTo(x => x.ReportsCountTo)
            .WithMessage("ReportsCountFrom cannot be greater than ReportsCountTo.")
            .WithErrorCode(IssueCodes.InvalidValue)
            .When(x => x.ReportsCountFrom.HasValue && x.ReportsCountTo.HasValue);
    }
}

using FS.Application.AuthLogic.DTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.MissingPetLogic.DTOs;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFFoundReportsQueryService(ApplicationDbContext context) : IFoundReportsQueryService
{
    public async Task<FoundReportDto[]> GetFoundReportsByAnnouncementIdAsync(
        Guid missingAnnouncementId,
        CancellationToken cancellationToken)
    {
        var result = await (from foundReport in context.FoundReports.AsNoTracking()
            join spottedUser in context.Users.AsNoTracking() on foundReport.FoundUserId equals spottedUser.Id
            where foundReport.MissingAnnouncementId == missingAnnouncementId
            select new FoundReportDto(
                foundReport.Id,
                new SpottedUserDto(
                    spottedUser.Id,
                    spottedUser.FullName.FirstName,
                    spottedUser.FullName.SecondName,
                    spottedUser.Contacts.Select(uc => new ContactData
                    {
                        ContactType = uc.Type,
                        Url = uc.Url,
                    }).ToArray()),
                foundReport.CreatedAt,
                foundReport.Images.Select(i => i.FullImagePath).ToArray()
            )).ToArrayAsync(cancellationToken);

        return result;
    }
}
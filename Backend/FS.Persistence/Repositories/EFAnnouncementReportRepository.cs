using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FS.Persistence.Repositories;

public class EFAnnouncementReportRepository(ApplicationDbContext context) : IAnnouncementReportRepository
{
    public async Task<bool> ExistsAsync(Guid announcementId, Guid reporterId, CancellationToken ct)
    {
        return await context.AnnouncementReports
            .AnyAsync(
                x => x.AnnouncementId == announcementId && x.ReporterId == reporterId,
                ct);
    }

    public async Task CreateAsync(AnnouncementReport report, CancellationToken ct)
    {
        context.AnnouncementReports.Add(report);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new DomainException(
                IssueCodes.NotUnique,
                "Пользователь уже пожаловался на это объявление.",
                nameof(report.AnnouncementId));
        }
    }
}

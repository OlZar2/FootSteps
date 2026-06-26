using FS.Application.MissingPetLogic.DTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IFoundReportsQueryService
{
    Task<FoundReportDto[]> GetFoundReportsByAnnouncementIdAsync(
        Guid missingAnnouncementId,
        CancellationToken cancellationToken);
}
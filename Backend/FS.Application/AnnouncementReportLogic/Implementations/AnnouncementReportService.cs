using FS.Application.AnnouncementReportLogic.DTOs;
using FS.Application.AnnouncementReportLogic.Interfaces;
using FS.Application.Shared.Exceptions;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.Exceptions;

namespace FS.Application.AnnouncementReportLogic.Implementations;

public class AnnouncementReportService(
    IAnimalAnnouncementRepository animalAnnouncementRepository,
    IAnnouncementReportRepository announcementReportRepository) : IAnnouncementReportService
{
    public async Task ReportAsync(ReportAnnouncementData data, CancellationToken ct)
    {
        var announcementExists = await animalAnnouncementRepository.ExistsByIdAsync(data.AnnouncementId, ct);
        if (!announcementExists)
        {
            throw new NotFoundException(nameof(AnimalAnnouncement), data.AnnouncementId);
        }

        var reportExists = await announcementReportRepository.ExistsAsync(
            data.AnnouncementId,
            data.ReporterId,
            ct);

        if (reportExists)
        {
            throw new DomainException(
                IssueCodes.NotUnique,
                "Пользователь уже пожаловался на это объявление.",
                nameof(data.AnnouncementId));
        }

        var report = AnnouncementReport.Create(
            data.AnnouncementId,
            data.ReporterId,
            data.Comment);

        await announcementReportRepository.CreateAsync(report, ct);
    }
}

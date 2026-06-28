using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.Exceptions;

namespace FS.Application.AnnouncementLogic.Policies;

public static class AnimalAnnouncementVisibilityPolicy
{
    public static void EnsureVisibleForPage(DeleteType? deleteType)
    {
        if (deleteType == DeleteType.AdminHide)
            throw new DomainException(
                IssueCodes.Announcement.DeletedByAdmin,
                "Объявление удалено по причинам модерации");
    }
}

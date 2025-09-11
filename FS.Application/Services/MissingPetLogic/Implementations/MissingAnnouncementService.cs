using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Core.Specifications;
using FS.Core.Stores;

namespace FS.Application.Services.MissingPetLogic.Implementations;

public class MissingAnnouncementService(IMissingAnnouncementRepository missingAnnouncementRepository) 
    : IMissingAnnouncementService
{
    public async Task<MissingAnnouncementFeed[]> GetFilteredMissingAnnouncementsByPageAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter)
    {
        var missingAnnouncementSpecification = new MissingAnnouncementSpecification
            (announcementFilter.District,
            announcementFilter.From,
            announcementFilter.Type,
            announcementFilter.Gender,
            null,
            a => a.Images);
        
        var feed = await missingAnnouncementRepository.
            GetFilteredMissingAnnouncementByPageAsync(lastDateTime, missingAnnouncementSpecification);

        var response =  feed.Select(a => new MissingAnnouncementFeed
        {
            Id = a.Id,
            PetName = a.PetName,
            CreatedAt = a.CreatedAt,
            District = a.District.Value,
            PetType = a.PetType,
            Gender = a.Gender,
            MainImagePath = a.Images[0].Path,
        }).ToArray();
        
        return response;
    }
}
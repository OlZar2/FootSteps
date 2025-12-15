using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.UserDomain.Entities;
using FS.Core.UserDomain.Stores;
using FS.Core.UserDomain.UserPolicies;
using FS.Core.UserDomain.ValueObjects;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.UserLogic.Implementations;

public class UserService(
    IUserRepository userRepository,
    IEditUserPolicy editUserPolicy,
    IImageStorageService imageStorageService,
    IImageQueryService imageQueryService,
    ITransactionFactory transactionFactory,
    IImageRepository imageRepository,
    IOptions<S3StorageConfiguration> s3Options
    ) : IUserService
{
    public async Task UpdateUserInfoAsync(Guid actorId, UpdateUserInfo userInfo, CancellationToken ct)
    {
        var user = await userRepository.GetByIdWithContactsAsync(userInfo.UserId, ct);
        
        var fullName = FullName.Create(
            userInfo.FirstName ?? user.FullName.FirstName,
            userInfo.SecondName ?? user.FullName.SecondName,
            userInfo.Patronymic ?? user.FullName.Patronymic);
        user.UpdateFullName(actorId, fullName, editUserPolicy);

        if (userInfo.UserContacts != null)
        {
            var initialContacts = userInfo.UserContacts
                .Select(uc => new InitialContact(uc.ContactType, uc.Url)).ToArray();
            user.UpdateContacts(actorId, initialContacts, editUserPolicy);
        }

        if (userInfo.Description != null)
        {
            user.UpdateDescription(actorId, userInfo.Description, editUserPolicy);
        }

        await userRepository.UpdateAsync(user, ct);
    }

    public async Task UpdateUserAvatarAsync(Guid actorId, UpdateUserAvatar updateUserAvatar, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var user = await userRepository.GetByIdAsync(updateUserAvatar.UserId, ct);

        FSImage? image = null;
        if (updateUserAvatar.AvatarId != null)
        {
            image = await imageRepository.GetByIdAsync(updateUserAvatar.AvatarId.Value, ct);
        }

        if (user.AvatarImageId != null)
        {
            var imageStorageKey = await imageQueryService.GetStorageKeyByImageId(user.AvatarImageId.Value, ct);
            await imageStorageService.DeleteAsync(imageStorageKey, ct);
        }
        
        user.UpdateAvatar(actorId, image?.Id, editUserPolicy);
        await userRepository.UpdateAsync(user, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task UpdateUserLocation(Guid userId, CoordinatesDto coordinates, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var point = geometryFactory.CreatePoint(new Coordinate(
            coordinates.Longitude,
            coordinates.Latitude));
        
        user.UpdateLastCoordinates(point);
        
        await userRepository.UpdateAsync(user, ct);
    }
    
    public async Task AddDevice(Guid userId, string deviceToken, CancellationToken ct)
    {
        var user = await userRepository.GetByIdWithDevicesAsync(userId, ct);
        var userDevice = UserDevice.Create(user, deviceToken);
        user.AddDevice(userDevice);
        await userRepository.UpdateAsync(user, ct);
    }
}
using FS.Application.AuthLogic.DTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Shared.Configurations;
using FS.Application.Shared.DTOs;
using FS.Application.Shared.Exceptions;
using FS.Application.UserLogic.DTOs;
using FS.Application.UserLogic.Interfaces;
using FS.Core.ImageDomain.Entities;
using FS.Core.UserDomain;
using FS.Core.UserDomain.Entities;
using FS.Core.UserDomain.Enums;
using FS.Core.UserDomain.Stores;
using FS.Core.UserDomain.UserPolicies;
using FS.Core.UserDomain.ValueObjects;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.UserLogic.Implementations;

public class UserService(
    IUserRepository userRepository,
    IEditUserPolicy editUserPolicy,
    IImageStorageService imageStorageService,
    IImageQueryService imageQueryService,
    ITransactionFactory transactionFactory,
    IUserQueryService userQueryService,
    IOptions<S3StorageConfiguration> s3Options
    ) : IUserService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3Options.Value;
    
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
        
        var user = await userRepository.GetByIdWithAvatarAsync(updateUserAvatar.UserId, ct);

        FSImage? image = null;
        var newAvatar = updateUserAvatar.Avatar;
        if (newAvatar != null)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = FSImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
            image = createdImage;
            await imageStorageService.UploadAsync(newAvatar.Content, s3Key, ct);
        }

        if (user.AvatarImage != null)
        {
            var imageStorageKey = await imageQueryService.GetStorageKeyByImageId(user.AvatarImage.Id, ct);
            await imageStorageService.DeleteAsync(imageStorageKey, ct);
        }
        
        user.UpdateAvatar(actorId, image, editUserPolicy);
        await userRepository.UpdateAsync(user, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task UpdateUserLocation(Guid userId, CoordinatesDto coordinates, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException(nameof(User), userId);
        
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var point = geometryFactory.CreatePoint(new Coordinate(
            coordinates.Longitude,
            coordinates.Latitude));
        
        user.UpdateLastCoordinates(point);
        
        await userRepository.UpdateAsync(user, ct);
    }
    
    public async Task AddDevice(Guid userId, string deviceToken, CancellationToken ct)
    {
        var usersWithThisActiveDevice = await userRepository.GetByActiveDeviceTokenWithDevicesAsync(deviceToken, ct);
        var userAddingDevice = await userRepository.GetByIdWithDevicesAsync(userId, ct);
        
        var userDevice = UserDevice.Create(userAddingDevice, deviceToken);
        foreach (var userWithActiveDevice in usersWithThisActiveDevice)
        {
            userWithActiveDevice.DeactivateDevice(userDevice);
        }
        userAddingDevice.AddOrActivateDevice(userDevice);
        
        await userRepository.UpdateAsync(userAddingDevice, ct);
    }

    public async Task AssignAdminRoleAsync(Guid userId, CancellationToken ct)
    {
        var user = await userRepository.GetByIdWithRolesAsync(userId, ct);

        user.AssignRole(Role.Admin);

        await userRepository.UpdateAsync(user, ct);
    }

    public async Task RemoveAdminRoleAsync(Guid userId, CancellationToken ct)
    {
        var user = await userRepository.GetByIdWithRolesAsync(userId, ct);

        user.RemoveRole(Role.Admin);

        await userRepository.UpdateAsync(user, ct);
    }

    public async Task<UserMainInfo> GetUserMainInfo(Guid userId, CancellationToken ct) =>
        await userQueryService.GetUserMainInfoByIdAsync(userId, ct);
}

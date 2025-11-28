using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.Policies.UserPolicies;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using FS.Core.ValueObjects.Contacts;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.UserLogic.Implementations;

public class UserService(
    IUserRepository userRepository,
    IEditUserPolicy editUserPolicy,
    IImageService imageService,
    ITransactionFactory transactionFactory) : IUserService
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
        
        var user = await userRepository.GetByIdWithAvatarAsync(updateUserAvatar.UserId, ct);
        
        var image = updateUserAvatar.Avatar != null ?
            await imageService.CreateImageAsync(updateUserAvatar.Avatar.Content, ct,
                nameof(updateUserAvatar.Avatar)) : null;

        if (user.AvatarImage != null)
        {
            await imageService.DeleteImageAsync(user.AvatarImage.Id, user.AvatarImage.Path, ct);
        }
        
        user.UpdateAvatar(actorId, image, editUserPolicy);
        
        await userRepository.UpdateAsync(user, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task UpdateUserLocation(Guid userId, CoordinatesDto coordinates, CancellationToken ct)
    {
        //TODO: аватар не нужен
        var user = await userRepository.GetByIdWithAvatarAsync(userId, ct);
        
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var point = geometryFactory.CreatePoint(new Coordinate(
            coordinates.Longitude,
            coordinates.Latitude));
        
        var coordinatesVO = CoordinatesVO.Create(coordinates.Latitude, coordinates.Longitude);
        user.UpdateLastCoordinates(point);
        
        await userRepository.UpdateAsync(user, ct);
    }
}
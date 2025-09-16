using FS.Application.DTOs.UserDTOs;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.Policies.UserPolicies;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using FS.Core.ValueObjects.Contacts;

namespace FS.Application.Services.UserLogic.Implementations;

public class UserService(
    IUserRepository userRepository,
    IEditUserPolicy editUserPolicy) : IUserService
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
}
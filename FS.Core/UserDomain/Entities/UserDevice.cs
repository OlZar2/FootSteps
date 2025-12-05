using System.ComponentModel.DataAnnotations;
using FS.Core.Shared.Abstractions;

namespace FS.Core.UserDomain.Entities;

public class UserDevice : Entity
{
    public Guid UserId { get; set; }

    public string DeviceToken { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    private UserDevice(
        Guid userId,
        string deviceToken,
        Guid? id = null
    ) : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        DeviceToken = deviceToken;
    } 

    public static UserDevice Create(
        User user,
        string deviceToken
    ) {
        if (string.IsNullOrWhiteSpace(deviceToken))
            throw new ValidationException("device token must not be empty");

        return new UserDevice(user.Id, deviceToken);
    }
    
    // EF
    private UserDevice() { } 
}
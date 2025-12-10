using FS.API.Services.ClaimLogic.Interfaces;

namespace FS.API.Services.ClaimLogic.Implementations;

public class ClaimService : IClaimService
{
    public Guid TryParseGuidClaim(string? guidClaim)
    {
        if (!Guid.TryParse(guidClaim, out var guid))
        {
            throw new ArgumentException("Некорректный Id пользователя");
        }

        return guid;
    }
}

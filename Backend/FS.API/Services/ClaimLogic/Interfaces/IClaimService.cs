namespace FS.API.Services.ClaimLogic.Interfaces;

public interface IClaimService
{
    Guid TryParseGuidClaim(string? guidClaim);
}

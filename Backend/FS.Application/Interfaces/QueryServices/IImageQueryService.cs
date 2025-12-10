namespace FS.Application.Interfaces.QueryServices;

public interface IImageQueryService
{
    Task<string> GetStorageKeyByImageId(Guid imageId, CancellationToken ct);
}
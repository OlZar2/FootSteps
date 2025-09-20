using System.Runtime.CompilerServices;
using FS.Application.DTOs.Shared;

namespace FS.API.Services.ImageLogic;

public class ImageService
{
    public async Task<FileData> GetFileInfo(IFormFile file, CancellationToken ct)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        return new FileData
        {
            Content = ms.ToArray()
        };
    }
    
    public async IAsyncEnumerable<FileData> GetFileInfo(IFormFile[] files, [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var file in files)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);

            yield return new FileData
            {
                Content = ms.ToArray()
            };
        }
    }
}
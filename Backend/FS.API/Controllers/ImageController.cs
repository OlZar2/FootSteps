using FS.Application.Services.ImageLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController(IImageService imageService) : ControllerBase
{
    [HttpPost("upload")]
    [Authorize]
    public async Task<Guid> Upload(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        
        return await imageService.UploadAsync(stream, ct);
    }
}
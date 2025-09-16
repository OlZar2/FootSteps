using FS.Application.Services.ImageLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/image")]
public class ImageController(IImageService imageService) : ControllerBase
{
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key, CancellationToken ct)
    {
        var response = await imageService.DownloadFileAsync(key, ct);
        return File(response.ResponseStream, response.MimeType);
    }
}
using FS.Application.Services.ImageLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

/// <summary>
/// Методы для работы с изображениями
/// </summary>
[ApiController]
[Route("api/image")]
public class ImageController(IImageService imageService) : ControllerBase
{
    /// <summary>
    /// Получение изображения
    /// </summary>
    /// <param name="key">название файла</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key, CancellationToken ct)
    {
        var response = await imageService.DownloadFileAsync(key, ct);
        return File(response.ResponseStream, response.MimeType);
    }
}
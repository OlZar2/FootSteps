using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.Search;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.SearchDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.SearchLogic.Interfaces;
using FS.Contracts.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(
    ISearchService searchService,
    IClaimService claimService,
    IValidator<SearchRequestModel> searchRequestModelValidator) : ControllerBase
{
    /// <summary>
    /// Создает запрос на поиск. Ответ с id запроса на поиск будет отправлен через signalR /hubs/search-announcements отправителю запроса
    /// </summary>
    [Authorize]
    [HttpPost("request")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task RequestSearch([FromForm] SearchRequestModel searchRequestModel, CancellationToken ct)
    {
        await searchRequestModelValidator.ValidateAndThrowAsync(searchRequestModel, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        await using var ms = new MemoryStream();
        await searchRequestModel.Image.CopyToAsync(ms, ct);

        var searchRequestDto = new SearchRequestDto
        {
            UserId = userId,
            Image = ms.ToArray(),
        };
        
        await searchService.RequestSearchAsync(searchRequestDto, ct);
    }
    
    /// <summary>
    /// Возвращает результаты поиска по id запроса на поиск 
    /// </summary>
    /// <param name="requestId">Id запрса на поиск</param>
    /// <param name="ct"></param>
    [Authorize]
    [ProducesResponseType(typeof(SearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [HttpGet("{requestId:guid}")]
    public async Task<SearchResultDto> GetById(Guid requestId, CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        return await searchService.GetSearchResultBySearchRequestId(requestId, userId, ct);
    }
    
    /// <summary>
    /// Возвращает список результатов поиска
    /// </summary>
    /// <param name="lastDateTime">Дата последнего полученного результата (для пагинации)</param>
    /// <param name="ct"></param>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(SearchResultDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<SearchResultDto[]> GetPaginated(DateTime lastDateTime, CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        return await searchService.GetPaginatedSearchResults(userId, lastDateTime, ct);
    }
}
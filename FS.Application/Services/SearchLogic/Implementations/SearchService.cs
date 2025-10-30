using System.Text.Json;
using FS.Application.DTOs.SearchDTOs;
using FS.Application.Interfaces;
using FS.Application.Interfaces.Events;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.SearchLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Entities;
using FS.Core.Exceptions;
using FS.Core.Stores;
using FS.Persistence.Repositories;
using FS.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FS.Application.Services.SearchLogic.Implementations;

public class SearchService(
    ISearchQueryService searchQueryService,
    ISearchRequestRepository searchRequestRepository,
    IHubContext<SearchAnnouncementsHub> searchAnnouncementsHub,
    IImageService imageService,
    IOutboxRepository outboxRepository,
    ITransactionService transactionService) : ISearchService
{
    public async Task DoSearch(Guid searchId, CancellationToken ct)
    {
        var searchRequest = await searchRequestRepository.GetByIdAsync(searchId, ct);
        var similarAnnouncements = await searchQueryService.DoVectorSearch(searchId, ct);
        
        searchRequest.SetResults(similarAnnouncements);
        await searchRequestRepository.UpdateAsync(searchRequest, ct);
        
        await searchAnnouncementsHub
            .Clients
            .User(searchRequest.CreatorId.ToString())
            .SendAsync("searchCompleted", new { requestId = searchRequest.Id }, cancellationToken: ct);
    }

    public async Task<SearchResultDto[]> GetPaginatedSearchResults(Guid userId, DateTime lastDateTime, CancellationToken ct)
        => await searchQueryService.GetSearchResults(userId, lastDateTime, ct);

    public async Task<SearchResultDto> GetSearchResultBySearchRequestId(
        Guid searchRequestId,
        Guid currentUserId,
        CancellationToken ct)
    {
        var searchRequest = await searchRequestRepository.GetByIdAsync(searchRequestId, ct);
        if (searchRequest.CreatorId != currentUserId)
            throw new NotEnoughRightsException(IssueCodes.AccessDenied ,"You cannot see other user searches");

        return await searchQueryService.GetSearchResultsBySearchRequestId(searchRequestId, ct);
    }

    public async Task RequestSearchAsync(SearchRequestDto searchRequestDto, CancellationToken ct) =>
        await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var path = await imageService.PutInS3(searchRequestDto.Image, ct);
            var searchRequest = SearchRequest.Create(path, searchRequestDto.UserId);

            await searchRequestRepository.AddAsync(searchRequest, ct);

            var outboxPayload = JsonSerializer.Serialize(new SearchRequestEvent(
                SearchId: searchRequest.Id.ToString(),
                ImageUrl: $"http://79.141.79.120:5000/api/image/{path}"
            ));
            var outboxEvent = OutboxEvent.Create("image.search.request", outboxPayload);
            await outboxRepository.AddAsync(outboxEvent, ct);
        }, ct);
}
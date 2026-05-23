using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.SearchLogic.DTOs;
using FS.Application.SearchLogic.Interfaces;
using FS.Application.Shared.Configurations;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.Exceptions;
using FS.Core.OutboxDomain.Entities;
using FS.Core.OutboxDomain.Stores;
using FS.Core.SearchDomain;
using FS.Core.SearchDomain.Stores;
using FS.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Pgvector;

namespace FS.Application.SearchLogic.Implementations;

public class SearchService(
    ISearchQueryService searchQueryService,
    ISearchRequestRepository searchRequestRepository,
    IHubContext<SearchAnnouncementsHub> searchAnnouncementsHub,
    IImageStorageService imageStorageService,
    IOutboxRepository outboxRepository,
    IImageRepository imageRepository,
    IOptions<S3StorageConfiguration> s3StorageConfigurationOptions,
    ITransactionFactory transactionFactory) : ISearchService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3StorageConfigurationOptions.Value;
    
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

    public async Task RequestSearchAsync(SearchRequestDto searchRequestDto, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var image = await imageRepository.GetByIdAsync(searchRequestDto.ImageId, ct);
        var searchRequest = SearchRequest.Create(image, searchRequestDto.UserId);

        await searchRequestRepository.AddAsync(searchRequest, ct);

        var outboxPayload = JsonSerializer.Serialize(new SearchRequestEvent(
            SearchId: searchRequest.Id.ToString(),
            ImageUrl: image.FullImagePath
        ));

        var outboxEvent = OutboxEvent.Create("image.search.request", outboxPayload);
        await outboxRepository.AddAsync(outboxEvent, ct);
        
        await transaction.CommitAsync(ct);
    }

    public async Task SetSearchEmbeddingAsync(Guid searchId, float[] vector, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var searchRequest = await searchRequestRepository.GetByIdAsync(searchId, ct);
        var pgVector = new Vector(vector);
        searchRequest.SetEmbedding(pgVector);

        var jobPayload = JsonSerializer.Serialize(new SearchOutboxEvent { SearchId = searchId });
        //TODO: возможно баг и не надо image.search.match
        var outboxEvent = OutboxEvent.Create("image.search.match", jobPayload);
        await outboxRepository.AddAsync(outboxEvent, ct);
        await searchRequestRepository.UpdateAsync(searchRequest, ct);
        
        await transaction.CommitAsync(ct);
    }
}
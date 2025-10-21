using System.Globalization;
using System.Text.Json;
using FS.API.Services.GeoLogic.Interfaces;
using FS.API.Services.GeoLogic.Options;
using FS.Application.DTOs.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FS.API.Services.GeoLogic.Implementations;

public class YandexGeocoder : IGeocoder
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<YandexGeocoder> _logger;

    public YandexGeocoder(
        HttpClient http,
        IOptions<YandexApiOptions> yandexApiOptions,
        ILogger<YandexGeocoder> logger)
    {
        _http = http;
        _http.BaseAddress ??= new Uri("https://geocode-maps.yandex.ru/v1/");
        if (_http.Timeout == default)
            _http.Timeout = TimeSpan.FromSeconds(15);

        _apiKey = yandexApiOptions.Value.ApiKey
                  ?? throw new InvalidOperationException("Yandex API key not set");

        _logger = logger;
    }

    public Task<string?> GetHouseOrNull(Coordinates point, CancellationToken ct) =>
        GetAddressComponentOrNull(point, componentKind: "house", ct);

    public Task<string?> GetStreetOrNull(Coordinates point, CancellationToken ct) =>
        GetAddressComponentOrNull(point, componentKind: "street", ct);

    public Task<string?> GetDistrictOrNull(Coordinates point, CancellationToken ct) =>
        GetAddressComponentOrNull(point, componentKind: "district", ct);

    public Task<string?> GetLocalityOrNull(Coordinates point, CancellationToken ct) =>
        GetAddressComponentOrNull(point, componentKind: "locality", ct);
    
    private async Task<string?> GetAddressComponentOrNull(
        Coordinates point,
        string componentKind,
        CancellationToken ct)
    {
        var uri = BuildQueryUri(
            kind: componentKind,
            geocode: string.Create(CultureInfo.InvariantCulture, $"{point.Longitude},{point.Latitude}"),
            results: "1");

        using var resp = await _http.GetAsync(uri, ct).ConfigureAwait(false);
        
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("Yandex geocoder returned HTTP {Status}", (int)resp.StatusCode);
            return null;
        }

        var json = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        try
        {
            using var doc = JsonDocument.Parse(json);
            var components = TryGetComponentsArray(doc.RootElement);
            if (components is null) return null;

            foreach (var comp in components.Value.EnumerateArray())
            {
                if (!comp.TryGetProperty("kind", out var kindEl) || kindEl.ValueKind != JsonValueKind.String)
                    continue;

                if (string.Equals(kindEl.GetString(), componentKind, StringComparison.OrdinalIgnoreCase)
                    && comp.TryGetProperty("name", out var nameEl)
                    && nameEl.ValueKind == JsonValueKind.String)
                {
                    return nameEl.GetString();
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse Yandex geocoder JSON.");
        }

        return null;
    }
    
    private string BuildQueryUri(string kind, string geocode, string results)
    {
        return QueryHelpers.AddQueryString("", new Dictionary<string, string?>
        {
            ["apikey"]  = _apiKey,
            ["format"]  = "json",
            ["kind"]    = kind,
            ["geocode"] = geocode,
            ["results"] = results
        });
    }
    
    private static JsonElement? TryGetComponentsArray(JsonElement root)
    {
        if (!root.TryGetProperty("response", out var response) ||
            !response.TryGetProperty("GeoObjectCollection", out var collection) ||
            !collection.TryGetProperty("featureMember", out var featureMember) ||
            featureMember.ValueKind != JsonValueKind.Array ||
            featureMember.GetArrayLength() == 0)
        {
            return null;
        }

        var first = featureMember[0];
        if (!first.TryGetProperty("GeoObject", out var geoObject) ||
            !geoObject.TryGetProperty("metaDataProperty", out var meta) ||
            !meta.TryGetProperty("GeocoderMetaData", out var geocoderMeta) ||
            !geocoderMeta.TryGetProperty("Address", out var address) ||
            !address.TryGetProperty("Components", out var components) ||
            components.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        return components;
    }
}
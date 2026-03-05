using FS.Application.DTOs.GeoDTOs;
using FS.Application.Interfaces.Geo;

namespace FS.Overpass;

public class OverpassGeoRequestService(HttpClient httpClient) : IGeoRequestService
{
    public async Task<DistrictInfo[]> GetDistrictInfoAsync(string cityName, CancellationToken ct)
    {
        var query = $"""
                     [out:json][timeout:60];
                     area["name"="{cityName}"]["boundary"="administrative"]->.cityArea;
                     (
                       relation(area.cityArea)
                         ["boundary"="administrative"]
                         ["admin_level"="9"]
                         ["name"];
                     );
                     out tags id;
                     """;

        using var content = new FormUrlEncodedContent(new Dictionary<string,string>
        {
            ["data"] = query
        });

        using var resp = await httpClient.PostAsync("https://overpass-api.de/api/interpreter", content, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        var data = System.Text.Json.JsonSerializer.Deserialize<OverpassResponse>(json);

        return (data?.elements ?? [])
            .Where(e => e.type == "relation" && e.tags != null && e.tags.TryGetValue("name", out _))
            .Select(e => new DistrictInfo(e.id, e.tags!["name"]))
            .Distinct()
            .ToArray();
    }
}
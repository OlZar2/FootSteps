using FS.Application.Shared.DTOs;
using NetTopologySuite.Geometries;

namespace FS.API.Controllers.Search.RequestModels;

public class SearchRequestModel
{
    public required IFormFile Image  { get; init; }
    public required CoordinatesDto Location { get; init; }
}
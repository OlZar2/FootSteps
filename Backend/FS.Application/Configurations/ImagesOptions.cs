namespace FS.Application.Configurations;

public class ImagesOptions
{
    public string[] AllowedImageExtensions { get; init; } = [];
    public string[] AllowedContentTypes { get; init; } = [];
    public required int MaxByteSize { get; init; }
}
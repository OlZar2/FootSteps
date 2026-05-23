namespace FS.Application.Shared.Configurations;

public record S3StorageConfiguration
{
    public required string ServiceURL { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public required string BucketName { get; init; }
    public required string Region { get; init; }
    public required string ImagesBucketUrl  { get; init; }
}

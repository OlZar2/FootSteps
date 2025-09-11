namespace FS.Application.Services.ImageLogic.Configurations;

public class S3StorageConfiguration
{
    public required string ServiceURL { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string BucketName { get; set; }
    public required string Region { get; set; }
}

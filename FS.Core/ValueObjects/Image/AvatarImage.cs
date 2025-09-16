namespace FS.Core.ValueObjects.Image;

public class AvatarImage
{
    public string S3Name { get; private set; }

    public AvatarImage(string s3Name)
    {
        S3Name = s3Name;
    }
}
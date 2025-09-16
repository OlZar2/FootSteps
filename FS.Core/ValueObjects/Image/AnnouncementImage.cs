namespace FS.Core.ValueObjects.Image;

public class AnnouncementImage
{
    public string S3Name { get; private set; }

    public AnnouncementImage(string s3Name)
    {
        S3Name = s3Name;
    }
}
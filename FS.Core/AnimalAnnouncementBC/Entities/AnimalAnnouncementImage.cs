using System.ComponentModel.DataAnnotations.Schema;
using FS.Core.Shared.Abstractions;
using Pgvector;

namespace FS.Core.AnimalAnnouncementBC.Entities;

public class AnimalAnnouncementImage : Entity
{
    public string S3Key { get; private set; }
    
    public string BucketURL { get; private set; }
    
    public string FullImagePath { get; private set; }
    
    [Column(TypeName = "vector(512)")]
    public Vector? Embedding { get; set; }

    private AnimalAnnouncementImage(string s3Key, string bucketURL, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        S3Key = s3Key;
        BucketURL = bucketURL;
        FullImagePath = $"{bucketURL}/{s3Key}";
    }

    public static AnimalAnnouncementImage Create(string s3Key, string bucketURL, Guid? id = null)
    {
        //TODO: подумать какое исключение бросать
        if (string.IsNullOrWhiteSpace(s3Key))
            throw new ArgumentException("Расширение не может быть пустым");
        

        return new AnimalAnnouncementImage(s3Key, bucketURL, id);
    }
    
    public void SetEmbedding(Vector embedding)
    {
        Embedding = embedding;
    }
    
    // EF
    private AnimalAnnouncementImage(){}
}
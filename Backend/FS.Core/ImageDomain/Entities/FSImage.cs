using System.ComponentModel.DataAnnotations.Schema;
using FS.Core.Shared.Abstractions;
using Pgvector;

namespace FS.Core.ImageDomain.Entities;

public class FSImage : Entity
{
    public string S3Key { get; private set; }
    
    public string BucketURL { get; private set; }
    
    public string FullImagePath { get; private set; }
    
    [Column(TypeName = "vector(512)")]
    public Vector? Embedding { get; set; }

    private FSImage(string s3Key, string bucketURL)
    {
        S3Key = s3Key;
        BucketURL = bucketURL;
        FullImagePath = $"{bucketURL}/{s3Key}";
    }

    public static FSImage Create(string s3Key, string bucketURL)
    {
        //TODO: подумать какое исключение бросать
        if (string.IsNullOrWhiteSpace(s3Key))
            throw new ArgumentException("Расширение не может быть пустым");
        

        return new FSImage(s3Key, bucketURL);
    }
    
    public void SetEmbedding(Vector? embedding)
    {
        Embedding = embedding;
    }
    
    // EF
    private FSImage(){}
}
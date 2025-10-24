namespace FS.RabbitMq.Options;

public class ImageEmbeddingRabbitOptions
{
    public string ExchangeName { get; init; }
    
    public string ResponseKey { get; init; }
    
    public string RequestKey { get; init; }
    
    public string ResponseQueue { get; init; }
}
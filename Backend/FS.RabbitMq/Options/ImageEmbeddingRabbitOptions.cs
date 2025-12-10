namespace FS.RabbitMq.Options;

public class ImageEmbeddingRabbitOptions
{
    public string ExchangeName { get; init; }
    
    public string ResponseKey { get; init; }
    public string RequestKey { get; init; }
    public string ResponseQueue { get; init; }
    
    public string SearchResponseKey { get; init; }
    public string SearchRequestKey { get; init; }
    public string SearchResponseQueue { get; init; }
}
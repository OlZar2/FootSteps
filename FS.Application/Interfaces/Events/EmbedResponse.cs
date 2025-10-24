namespace FS.Application.Interfaces.Events;

public record EmbedResponse(string ImageId, float[]? Embedding);
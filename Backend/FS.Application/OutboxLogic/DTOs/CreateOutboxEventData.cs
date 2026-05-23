namespace FS.Application.OutboxLogic.DTOs;

public class CreateOutboxEventData
{
    public string Type { get; set; }
    public string Payload  { get; set; }
}
namespace FS.Application.DTOs.OutboxDto;

public class CreateOutboxEventData
{
    public string Type { get; set; }
    public string Payload  { get; set; }
}
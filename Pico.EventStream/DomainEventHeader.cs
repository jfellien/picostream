namespace Pico.EventStream;

public class DomainEventHeader
{
    public required string RequesterId { get; set; }
    public required string TracingId { get; set; }
}
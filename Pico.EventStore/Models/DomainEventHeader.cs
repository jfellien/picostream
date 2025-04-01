namespace Pico.EventStore.Models;

public class DomainEventHeader
{
    public required string RequesterId { get; set; }
    public required string TracingId { get; set; }
}
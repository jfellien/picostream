namespace Pico.EventStore.Models;

public abstract class DomainEvent(string requesterId) : IDomainEvent
{
    public DomainEventHeader Header { get; set; } = new()
    {
        TracingId = Guid.NewGuid().ToString(),
        RequesterId = requesterId
    };
}
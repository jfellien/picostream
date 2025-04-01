namespace Pico.EventStore.Models;

public interface IDomainEvent
{
    DomainEventHeader Header { get; set; }
}
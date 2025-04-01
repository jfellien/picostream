namespace Pico.EventStream.Models;

public interface IDomainEvent
{
    DomainEventHeader Header { get; set; }
}
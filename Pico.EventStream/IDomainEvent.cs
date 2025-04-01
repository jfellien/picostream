namespace Pico.EventStream;

public interface IDomainEvent
{
    DomainEventHeader Header { get; set; }
}
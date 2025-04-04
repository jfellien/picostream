namespace Pico.EventSource.Models;

public class DomainEventStream : List<IDomainEvent>
{
    public bool IsEmpty => Count == 0;
}
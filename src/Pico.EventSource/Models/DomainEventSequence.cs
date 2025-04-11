namespace Pico.EventSource.Models;

public class DomainEventSequence : List<SequencedDomainEvent>
{
    public bool HasBeenSequenced { get; internal set; }
}
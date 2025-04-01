namespace Pico.EventStream.Models;

public class DomainEventSequence : List<SequencedDomainEvent>
{
    public bool HasBeenSequenced { get; internal set; }
}
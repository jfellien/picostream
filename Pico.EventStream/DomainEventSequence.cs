namespace Pico.EventStream;

public class DomainEventSequence : List<SequencedDomainEvent>
{
    public bool HasBeenSequenced { get; internal set; }
}
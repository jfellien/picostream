namespace Pico.EventStream;

public record SequencedDomainEvent(long SequenceNumber, IDomainEvent Instance);
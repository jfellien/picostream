namespace Pico.EventStore.Models;

public record SequencedDomainEvent(long SequenceNumber, IDomainEvent Instance);
namespace Pico.EventSource.Models;

public record SequencedDomainEvent(long SequenceNumber, IDomainEvent Instance);
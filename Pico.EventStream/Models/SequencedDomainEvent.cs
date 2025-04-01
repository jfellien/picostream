namespace Pico.EventStream.Models;

public record SequencedDomainEvent(long SequenceNumber, IDomainEvent Instance);
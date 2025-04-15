using Pico.EventSource.Models;

namespace Pico.EventSource.Persistence;

public class DomainEventEnvelope
{
    public DateTimeOffset TimeStamp { get; set; }
    
    public required string Context { get; set; }
    public string StreamId { get; set; }
    public Guid EventId { get; set; }
    
    public required IDomainEvent Event { get; set; }
}
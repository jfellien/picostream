using System.Text.Json.Serialization;

namespace Pico.EventSource.Models;

[method: JsonConstructor]
public class DomainEventHeader(string context, Guid streamId)
{
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    public string Context { get; set; } = context;
    public Guid StreamId { get; set; } = streamId;
    public Guid EventId { get; set; } = Guid.NewGuid();
}
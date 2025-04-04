using System.Text.Json.Serialization;

namespace Pico.EventSource.Models;

[method: JsonConstructor]
public class DomainEvent() : IDomainEvent
{
    //public DomainEventHeader Header { get; set; } = new(context, streamId);
}
using System.Collections.Immutable;
using Pico.EventSource.Models;
using Pico.EventSource.Persistence;

namespace Pico.EventSource;

public sealed class DomainEventStream
{
    private readonly IEventStore _eventStore;

    internal DomainEventStream(IEventStore eventStore, ContextId contextId, StreamId streamId, List<IDomainEvent> events)
    {
        _eventStore = eventStore;
        
        ContextId = contextId;
        StreamId = streamId;
        Events = events.ToImmutableList();
    }

    public ContextId ContextId { get; }
    public StreamId StreamId { get; }
    public ImmutableList<IDomainEvent> Events { get; }
    public bool IsEmpty => Events.Count == 0;

    public Task Append(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        return _eventStore.Append(ContextId, StreamId, domainEvent, ct);
    }
}
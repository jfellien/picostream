using Pico.EventSource.Models;
using Pico.EventSource.Persistence;

namespace Pico.EventSource;

/// <summary>
/// Represents a stream of domain events specific to a context, entity, and entity ID,
/// with functionality to append and retrieve events in an organized manner.
/// </summary>
public class EventStore : IEventStore
{
    private readonly IReadAndWriteDomainEvents _storage;
    
    /// <summary>
    /// Represents a stream of domain events associated with a specific context, entity, and entity ID.
    /// </summary>
    public EventStore(IReadAndWriteDomainEvents storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Retrieves a stream of domain events for the specified context and stream ID.
    /// </summary>
    /// <param name="contextId">The context identifier for which the domain event stream is to be retrieved.</param>
    /// <param name="streamId">The unique identifier of the event stream to retrieve.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DomainEventStream"/> representing the retrieved domain events.</returns>
    public async Task<DomainEventStream> GetStream(ContextId contextId, StreamId streamId, CancellationToken ct = default)
    {
        IEnumerable<IDomainEvent> domainEvents = await _storage.Read(contextId, streamId, ct);

        return new DomainEventStream(this, contextId, streamId, domainEvents.ToList());
    }

    /// <summary>
    /// Appends a domain event to the specified event stream associated with a given contextId and stream ID.
    /// </summary>
    /// <param name="contextId">The contextId associated with the entity and event stream.</param>
    /// <param name="streamId">The unique identifier of the event stream where the domain event is appended.</param>
    /// <param name="domainEvent">The domain event to be appended to the stream.</param>
    /// <param name="ct">A cancellation token to observe while appending the event.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    public async Task Append(ContextId contextId, StreamId streamId, IDomainEvent domainEvent, CancellationToken ct = default)
    {
        await _storage.Write(domainEvent, contextId, streamId, ct);
    }
}
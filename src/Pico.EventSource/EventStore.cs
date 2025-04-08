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
    private readonly DomainEventStream _history;

    /// <summary>
    /// Represents a stream of domain events associated with a specific context, entity, and entity ID.
    /// </summary>
    public EventStore(IReadAndWriteDomainEvents storage)
    {
        _storage = storage;

        _history = [];
    }

    /// <summary>
    /// Retrieves a stream of domain events for the specified context and stream ID.
    /// </summary>
    /// <param name="context">The context identifier for which the domain event stream is to be retrieved.</param>
    /// <param name="streamId">The unique identifier of the event stream to retrieve.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DomainEventStream"/> representing the retrieved domain events.</returns>
    public async Task<DomainEventStream> GetStream(string context, Guid streamId, CancellationToken ct = default)
    {
        if (_history.IsEmpty == false)
        {
            return _history;
        }

        DomainEventStream eventStream = await _storage.Read(context, streamId, ct);

        _history.AddRange(eventStream);

        return eventStream;
    }

    /// <summary>
    /// Appends a domain event to the specified event stream associated with a given context and stream ID.
    /// </summary>
    /// <param name="context">The context associated with the entity and event stream.</param>
    /// <param name="streamId">The unique identifier of the event stream where the domain event is appended.</param>
    /// <param name="domainEvent">The domain event to be appended to the stream.</param>
    /// <param name="ct">A cancellation token to observe while appending the event.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    public async Task Append(string context, Guid streamId, IDomainEvent domainEvent, CancellationToken ct = default)
    {
        await _storage.Write(domainEvent, context, streamId, ct);

        _history.Add(domainEvent);
    }
}
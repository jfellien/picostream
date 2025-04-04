using Pico.EventSource.Models;
using Pico.EventSource.Persistence;

namespace Pico.EventSource;

/// <summary>
/// Represents a stream of domain events specific to a context, entity, and entity ID,
/// with functionality to append and retrieve events in an organized manner.
/// </summary>
public class EventStore
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
    /// Retrieves a stream of domain events for a specified context and stream ID.
    /// If events already exist in the in-memory history, they will be included in the returned stream.
    /// Otherwise, events are fetched from the underlying storage and loaded into history.
    /// </summary>
    /// <param name="context">The context identifier for which the stream of domain events is retrieved.</param>
    /// <param name="streamId">The unique identifier of the event stream.</param>
    /// <returns>A <see cref="DomainEventStream"/> containing the domain events associated with the specified context and stream ID.</returns>
    public async Task<DomainEventStream> GetStream(string context, Guid streamId)
    {
        DomainEventStream domainEventStream = [];

        if (_history.IsEmpty == false)
        {
            return _history;
        }

        DomainEventStream eventStream = await _storage.Read(context, streamId, CancellationToken.None);
        
        _history.AddRange(eventStream);
            
        return eventStream;
    }

    /// <summary>
    /// Appends a domain event to the event stream and persists it.
    /// </summary>
    /// <param name="domainEvent">The domain event to append to the stream.</param>
    /// <param name="ct">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    public async Task Append(IDomainEvent domainEvent, string context, Guid streamId, CancellationToken ct = default)
    {
        await _storage.Write(domainEvent, context, streamId, ct);
        
        _history.Add(domainEvent);
    }
}
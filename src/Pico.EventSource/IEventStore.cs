using Pico.EventSource.Models;

namespace Pico.EventSource;

public interface IEventStore
{
    /// <summary>
    /// Retrieves a stream of domain events for a specified context and stream ID.
    /// If events already exist in the in-memory history, they will be included in the returned stream.
    /// Otherwise, events are fetched from the underlying storage and loaded into history.
    /// </summary>
    /// <param name="context">The context identifier for which the stream of domain events is retrieved.</param>
    /// <param name="streamId">The unique identifier of the event stream.</param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="DomainEventStream"/> containing the domain events associated with the specified context and stream ID.</returns>
    Task<DomainEventStream> GetStream(string context, Guid streamId, CancellationToken ct = default);

    /// <summary>
    /// Appends a domain event to a specific event stream identified by the context and stream ID.
    /// </summary>
    /// <param name="context">The context identifier representing the namespace or domain to which the event belongs.</param>
    /// <param name="streamId">The unique identifier of the event stream where the event will be appended.</param>
    /// <param name="domainEvent">The domain event to append to the specified stream.</param>
    /// <param name="ct">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    Task Append(string context, Guid streamId, IDomainEvent domainEvent, CancellationToken ct = default);
}
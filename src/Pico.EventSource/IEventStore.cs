using Pico.EventSource.Models;

namespace Pico.EventSource;

public interface IEventStore
{
    /// <summary>
    /// Retrieves a stream of domain events associated with a specific event stream within a given context.
    /// </summary>
    /// <param name="contextId">The identifier representing the context or domain namespace from which the event stream is retrieved.</param>
    /// <param name="streamId">The unique identifier of the specific event stream containing the domain events.</param>
    /// <param name="ct">The token allowing the asynchronous operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DomainEventStream"/> with the domain events for the specified context and stream.</returns>
    Task<DomainEventStream> GetStream(ContextId contextId, StreamId streamId, CancellationToken ct = default);

    /// <summary>
    /// Appends a domain event to a specified event stream within a given context.
    /// The event is persisted into the event store for the particular context and stream.
    /// </summary>
    /// <param name="contextId">The identifier representing the context or domain namespace where the event occurs.</param>
    /// <param name="streamId">The identifier of the event stream to which the event will be appended.</param>
    /// <param name="domainEvent">The specific domain event to add to the event stream.</param>
    /// <param name="ct">The token allowing the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous append operation.</returns>
    Task Append(ContextId contextId, StreamId streamId, IDomainEvent domainEvent, CancellationToken ct = default);
}
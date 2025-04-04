using Pico.EventSource.Models;

namespace Pico.EventSource.Persistence;

/// <summary>
/// Defines an interface for reading and writing domain events in a persistent store,
/// supporting operations for retrieving and appending domain events associated with
/// specific contexts, entities, and entity identifiers.
/// </summary>
public interface IReadAndWriteDomainEvents
{
    /// <summary>
    /// Reads domain events from the persistent store for the specified context and stream identifier.
    /// </summary>
    /// <param name="context">The context associated with the domain events to be read.</param>
    /// <param name="streamId">The unique identifier of the stream for which domain events are to be retrieved.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="DomainEventStream"/> collection with the retrieved domain events.</returns>
    Task<DomainEventStream> Read(string context, Guid streamId, CancellationToken ct = default);

    /// <summary>
    /// Writes a domain event to the persistent store with the specified context and stream identifier.
    /// </summary>
    /// <param name="domainEvent">The domain event to be written to the persistent store.</param>
    /// <param name="context">The context associated with the domain event.</param>
    /// <param name="streamId">The unique stream identifier associated with the domain event.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Write(IDomainEvent domainEvent, string context, Guid streamId, CancellationToken ct = default);
}
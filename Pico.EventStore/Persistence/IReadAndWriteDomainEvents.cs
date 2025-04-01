using Pico.EventStore.Models;

namespace Pico.EventStore.Persistence;

/// <summary>
/// Defines an interface for reading and writing domain events in a persistent store,
/// supporting operations for retrieving and appending domain events associated with
/// specific contexts, entities, and entity identifiers.
/// </summary>
public interface IReadAndWriteDomainEvents
{
    /// <summary>
    /// Reads a sequence of domain events filtered by the specified context.
    /// </summary>
    /// <param name="context">The context to filter the domain events by.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the sequence of domain events associated with the specified context.</returns>
    Task<DomainEventSequence> ReadBy(string context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a sequence of domain events filtered by the specified context and entity.
    /// </summary>
    /// <param name="context">The context to filter the domain events by.</param>
    /// <param name="entity">The entity to filter the domain events by within the specified context.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the sequence of domain events associated with the specified context and entity.</returns>
    Task<DomainEventSequence> ReadBy(string context, string entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a sequence of domain events filtered by the specified context.
    /// </summary>
    /// <param name="context">The context to filter the domain events by.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the sequence of domain events associated with the specified context.</returns>
    Task<DomainEventSequence> ReadBy(string context, string entity, string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a domain event to the persistent store with the specified context, entity, and entity identifier.
    /// </summary>
    /// <param name="domainEvent">The domain event to write to the persistent store.</param>
    /// <param name="context">The context associated with the domain event.</param>
    /// <param name="entity">The entity associated with the domain event, if applicable.</param>
    /// <param name="entityId">The entity identifier associated with the domain event, if applicable.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the sequence number of the written domain event.</returns>
    Task<long> Write(IDomainEvent domainEvent, string context, string? entity = null, string? entityId = null,
        CancellationToken cancellationToken = default);
}
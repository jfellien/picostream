using Pico.EventStream.Models;

namespace Pico.EventStream.LinQ.Extensions;

/// <summary>
/// Provides extension methods for working with a stream of domain events.
/// Includes utilities to filter or verify the occurrence of specific event types within the stream.
/// </summary>
public static class EventStreamExtensions
{
    /// <summary>
    /// Retrieves a filtered collection of domain events of a specified type from the given stream of events.
    /// </summary>
    /// <param name="domainEvents">The stream of events to filter, represented as an IEnumerable.</param>
    /// <typeparam name="T">The type of domain events to retrieve from the stream, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>An <see cref="ItemsFilter{T}"/> object that provides mechanisms to filter and retrieve domain events of the specified type.</returns>
    public static ItemsFilter<T> Get<T>(this IEnumerable<object> domainEvents)
        where T : IDomainEvent
    {
        return new ItemsFilter<T>(domainEvents);
    }


    /// <summary>
    /// Provides verification tools to analyze a stream of domain events for specific occurrences
    /// or sequences involving events of a specified type.
    /// </summary>
    /// <param name="domainEvents">The stream of events to analyze, represented as an IEnumerable.</param>
    /// <typeparam name="T">The type of domain events to verify within the stream, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>A <see cref="Verification{T}"/> object that provides mechanisms to check for the occurrence, order, or other properties of domain events of the specified type.</returns>
    public static Verification<T> Event<T>(this IEnumerable<object> domainEvents)
        where T : IDomainEvent
    {
        return new Verification<T>(domainEvents);
    }
}
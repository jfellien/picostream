using Pico.EventSource.Models;

namespace Pico.EventSource.LinQ.Extensions;

/// <summary>
/// Provides extension methods for working with a stream of domain events.
/// Includes utilities to filter or verify the occurrence of specific event types within the stream.
/// </summary>
public static class EventStreamExtensions
{
    /// <summary>
    /// Provides verification tools to analyze a stream of domain events for specific occurrences
    /// or sequences involving events of a specified type.
    /// </summary>
    /// <param name="domainEventStream">The stream of events to analyze, represented as an IEnumerable.</param>
    /// <typeparam name="T">The type of domain events to verify within the stream, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>A <see cref="Verification{T}"/> object that provides mechanisms to check for the occurrence, order, or other properties of domain events of the specified type.</returns>
    public static Verification<T> VerifyIf<T>(this DomainEventStream domainEventStream)
        where T : IDomainEvent
    {
        return new Verification<T>(domainEventStream.Events);
    }

    /// <summary>
    /// Retrieves the first domain event of a specified type from the given stream of domain events.
    /// </summary>
    /// <param name="domainEventStream">The stream of domain events to search, represented as a DomainEvents object.</param>
    /// <typeparam name="T">The type of domain event to retrieve, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>A <see cref="FirstSingleItemFilter{T}"/> object that enables further filtering or retrieval of the first matching domain event.</returns>
    public static FirstSingleItemFilter<T> First<T>(this DomainEventStream domainEventStream)
        where T : IDomainEvent
    {
        return new FirstSingleItemFilter<T>(domainEventStream.Events);
    }

    /// <summary>
    /// Retrieves the last occurrence of a domain event of a specific type from the given domain events stream.
    /// </summary>
    /// <param name="domainEventStream">The stream of domain events to evaluate, represented as a <see cref="DomainEventStream"/> collection.</param>
    /// <typeparam name="T">
    /// The type of domain event to retrieve from the stream, which must implement <see cref="IDomainEvent"/>.
    /// </typeparam>
    /// <returns>
    /// A <see cref="LastSingleItemFilter{T}"/> object that allows querying the stream to obtain the last event of the specified type.
    /// </returns>
    public static LastSingleItemFilter<T> Last<T>(this DomainEventStream domainEventStream)
        where T : IDomainEvent
    {
        return new LastSingleItemFilter<T>(domainEventStream.Events);
    }

    /// <summary>
    /// Retrieves the last occurrence of a specified domain event type from the domain event stream.
    /// </summary>
    /// <param name="domainEventStream">The stream of domain events to search through, represented as an IEnumerable.</param>
    /// <typeparam name="T">The type of the domain event to retrieve, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="InvalidOperationException">The source sequence is empty</exception>
    /// <returns>The last instance of the specified domain event type present in the stream.</returns>
    public static T LastItem<T>(this DomainEventStream domainEventStream)
        where T : IDomainEvent
    {
        return domainEventStream.Events.OfType<T>().Last();
    }
    
    /// <summary>
    /// Retrieves a single event of the specified type from the domainEvents stream, ensuring there is only one such event.
    /// </summary>
    /// <param name="domainEventStream">The stream of domainEvents to search, represented as a <see cref="DomainEventStream"/> object.</param>
    /// <typeparam name="T">The type of domain event to retrieve, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>The single event of type T if exactly one such event exists, or null if no matching events are found or if multiple events of the specified type exist.</returns>
    public static T? Single<T>(this DomainEventStream domainEventStream) 
        where T : IDomainEvent
    {
        return domainEventStream.Events.OfType<T>().SingleOrDefault();
    }

    /// <summary>
    /// Retrieves a single domain event of a specified type from the given domainEvents that matches the provided filter expression.
    /// </summary>
    /// <param name="domainEventStream">The domain events to filter, represented as an instance of <see cref="DomainEventStream"/>.</param>
    /// <param name="filterExpression">A predicate function used to filter the domain events of the specified type.</param>
    /// <typeparam name="T">The type of domain events to retrieve from the domainEvents, which must implement <see cref="IDomainEvent"/>.</typeparam>
    /// <returns>The single domain event of the specified type that matches the filter expression, or null if no event matches.</returns>
    public static T? Single<T>(this DomainEventStream domainEventStream, Func<T, bool> filterExpression)
        where T : IDomainEvent
    {
        return domainEventStream.Events.OfType<T>().SingleOrDefault(filterExpression);
    }
    
}
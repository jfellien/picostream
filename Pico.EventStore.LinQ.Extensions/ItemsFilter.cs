using Pico.EventStore.Models;

namespace Pico.EventStore.LinQ.Extensions;

/// <summary>
/// Provides a mechanism to filter and retrieve domain events of type TSource from a stream of events.
/// </summary>
/// <typeparam name="TSource">The type of domain event to filter, which must implement <see cref="IDomainEvent"/>.</typeparam>
/// <remarks>
/// This class supports filtering, retrieval of specific domain events, and allows for querying patterns such as first, last, any, and all events.
/// </remarks>
public class ItemsFilter<TSource>(IEnumerable<object> sourceStream)
    where TSource : IDomainEvent
{
    /// <summary>
    /// Filters the source stream based on the provided expression and retrieves a single matching event of type TSource.
    /// </summary>
    /// <param name="filterExpression">The filter function used to evaluate each event in the source stream.</param>
    /// <returns>The event of type TSource that matches the filter criteria, or null if no such event exists or if there is more than one match.</returns>
    public TSource? Where(Func<TSource, bool> filterExpression)
    {
        return sourceStream.OfType<TSource>().SingleOrDefault(filterExpression);
    }

    /// <summary>
    /// Retrieves the first domain event of type TSource from the source stream.
    /// </summary>
    /// <returns>An instance of FirstSingleItemFilter which can be used to further filter or retrieve the first event.</returns>
    public FirstSingleItemFilter<TSource> First()
    {
        return new FirstSingleItemFilter<TSource>(sourceStream);
    }

    /// <summary>
    /// Retrieves the last domain event of type TSource from the source stream.
    /// </summary>
    /// <returns>An instance of LastSingleItemFilter which can be used to further filter or retrieve the last event.</returns>
    public LastSingleItemFilter<TSource> Last()
    {
        return new LastSingleItemFilter<TSource>(sourceStream);
    }

    /// <summary>
    /// Retrieves the only domain event of type TSource from the source stream, with the expectation that there is exactly one match.
    /// </summary>
    /// <returns>The single event of type TSource from the source stream, or null if no match is found or if more than one element exists.</returns>
    public TSource? TheOnlyOne()
    {
        return sourceStream.OfType<TSource>().SingleOrDefault();
    }

    /// <summary>
    /// Retrieves any domain event of type TSource from the source stream.
    /// </summary>
    /// <returns>An instance of AnyItemFilter which can be used to further filter or retrieve events of type TSource.</returns>
    public AnyItemFilter<TSource> Any()
    {
        return new AnyItemFilter<TSource>(sourceStream);
    }

    /// <summary>
    /// Retrieves all domain events of type TSource from the source stream.
    /// </summary>
    /// <returns>A collection of all domain events of type TSource from the source stream.</returns>
    public IEnumerable<TSource> All()
    {
        return sourceStream.OfType<TSource>();
    }
}
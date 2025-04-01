using Pico.EventStore.Models;

namespace Pico.EventStore.LinQ.Extensions;

/// <summary>
/// Provides a method to retrieve the last domain event of a specific type from a stream of events
/// and apply an optional filtering criterion.
/// </summary>
/// <typeparam name="TSource">
/// The type of domain event to retrieve, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
/// <remarks>
/// This class allows querying a stream of objects to retrieve the last domain event
/// of type <typeparamref name="TSource"/>, with support for filtering based on a predicate.
/// </remarks>
public class LastSingleItemFilter<TSource>(IEnumerable<object> sourceStream)
    where TSource : IDomainEvent
{
    /// <summary>
    /// Filters the source stream of events to retrieve the last domain event of a specific type
    /// that matches the specified filtering criterion.
    /// </summary>
    /// <param name="filterExpression">
    /// A function to test each domain event for a condition. Only events for which this predicate returns true
    /// are considered when retrieving the last domain event.
    /// </param>
    /// <returns>
    /// The last domain event of type <typeparamref name="TSource"/> that matches the specified filter,
    /// or <c>null</c> if none are found.
    /// </returns>
    public TSource? Where(Func<TSource, bool> filterExpression)
    {
        return sourceStream.OfType<TSource>().LastOrDefault(filterExpression);
    }
}
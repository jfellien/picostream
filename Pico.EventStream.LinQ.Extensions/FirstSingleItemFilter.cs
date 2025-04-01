using Pico.EventStream.Models;

namespace Pico.EventStream.LinQ.Extensions;

/// <summary>
/// Provides a mechanism to retrieve the first domain event of type TSource from a stream of events
/// and apply an additional filter to determine the specific event to retrieve.
/// </summary>
/// <typeparam name="TSource">The type of domain event to retrieve, which must implement <see cref="IDomainEvent"/>.</typeparam>
/// <remarks>
/// This class enables filtering and retrieval of the first domain event that matches a given condition in a stream.
/// If no matching event is found, the result is null.
/// </remarks>
public class FirstSingleItemFilter<TSource>(IEnumerable<object> sourceStream)
    where TSource : IDomainEvent
{
    /// <summary>
    /// Filters the source stream of events and returns the first domain event
    /// of type <typeparamref name="TSource"/> that matches the specified filter condition,
    /// or null if no event matches the condition.
    /// </summary>
    /// <param name="filterExpression">
    /// A function to test each domain event for a condition. The function returns true for the event that satisfies the condition.
    /// </param>
    /// <returns>
    /// The first domain event of type <typeparamref name="TSource"/> that satisfies the filter condition,
    /// or null if no such event exists in the source stream.
    /// </returns>
    public TSource? Where(Func<TSource, bool> filterExpression)
    {
        return sourceStream.OfType<TSource>().FirstOrDefault(filterExpression);
    }
}
using Pico.EventStore.Models;

namespace Pico.EventStore.LinQ.Extensions;

/// <summary>
/// Provides a mechanism to filter and retrieve multiple domain events of type TSource from a stream of events.
/// </summary>
/// <typeparam name="TSource">The type of domain event to filter, which must implement <see cref="IDomainEvent"/>.</typeparam>
/// <remarks>
/// This class supports filtering domain events by applying a specified condition using the provided filter expression.
/// It allows for obtaining a collection of events of type TSource that satisfy the given conditions.
/// </remarks>
public class AnyItemFilter<TSource>(IEnumerable<object> sourceStream)
    where TSource : IDomainEvent
{
    /// <summary>
    /// Filters the source stream of objects and returns an enumerable of items of type TSource
    /// that satisfy the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">
    /// A function that defines the conditions the items of type TSource must satisfy.
    /// </param>
    /// <returns>
    /// A collection of items of type TSource from the source stream that satisfy the given filter expression.
    /// </returns>
    public IEnumerable<TSource> Where(Func<TSource, bool> filterExpression)
    {
        return sourceStream.OfType<TSource>().Where(filterExpression);
    }
}
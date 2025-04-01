using Pico.EventStore.Models;

namespace Pico.EventStore.LinQ.Extensions;

/// <summary>
/// Represents a mechanism to check whether a domain event of type <typeparamref name="TLater"/>
/// occurs later than an event of type <typeparamref name="TEarlier"/> within a given source stream.
/// </summary>
/// <typeparam name="TEarlier">
/// The type of the earlier domain event, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
/// <typeparam name="TLater">
/// The type of the later domain event to compare against, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
public class HappenedEarlierVerification<TEarlier, TLater>(IEnumerable<object> sourceStream, TEarlier earlierEvent)
    where TLater : IDomainEvent
    where TEarlier : IDomainEvent
{
    /// <summary>
    /// Determines whether a domain event of type <typeparamref name="TLater"/> occurs later
    /// than an earlier event of type <typeparamref name="TEarlier"/> in the provided source stream.
    /// </summary>
    /// <param name="filterExpression">
    /// A function to filter the domain events of type <typeparamref name="TLater"/> based on specific conditions.
    /// </param>
    /// <returns>
    /// A boolean indicating whether the event of type <typeparamref name="TLater"/> occurs later than
    /// the earlier event of type <typeparamref name="TEarlier"/> in the source stream.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if no domain event of type <typeparamref name="TLater"/> satisfying
    /// the provided filter expression is found in the source stream.
    /// </exception>
    public bool Where(Func<TLater, bool> filterExpression)
    {
        int indexOfEarlierEvent = sourceStream.ToList().IndexOf(earlierEvent);

        TLater? laterEvent = sourceStream.OfType<TLater>().LastOrDefault(filterExpression);

        if (laterEvent is null)
        {
            throw new ArgumentException($"Can't find Event of type {typeof(TLater)} that fits to given filter expression.");
        }

        int indexOfLaterEvent = sourceStream.ToList().IndexOf(laterEvent);

        return indexOfLaterEvent > indexOfEarlierEvent;
    }
}
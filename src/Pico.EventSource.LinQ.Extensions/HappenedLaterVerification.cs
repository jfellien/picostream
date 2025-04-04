using Pico.EventSource.Models;

namespace Pico.EventSource.LinQ.Extensions;

/// <summary>
/// Represents a mechanism to verify whether a specific event of type <typeparamref name="TEarlier"/>
/// occurred earlier in the source stream than a designated event of type <typeparamref name="TLater"/>.
/// </summary>
/// <typeparam name="TEarlier">
/// The type of the earlier domain event to check, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
/// <typeparam name="TLater">
/// The type of the later domain event being compared, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
public class HappenedLaterVerification<TEarlier, TLater>(IEnumerable<object> sourceStream, TLater laterEvent)
    where TLater : IDomainEvent
    where TEarlier : IDomainEvent
{
    /// <summary>
    /// Determines whether an event of type <typeparamref name="TEarlier"/> that satisfies the provided filter expression
    /// occurred earlier in the source stream than the specified event of type <typeparamref name="TLater"/>.
    /// </summary>
    /// <param name="filterExpression">
    /// A function that defines the filter criteria to identify the relevant event of type <typeparamref name="TEarlier"/>.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if an event of type <typeparamref name="TEarlier"/> that meets the filter criteria occurs earlier
    /// in the source stream than the event of type <typeparamref name="TLater"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when no event of type <typeparamref name="TEarlier"/> satisfies the provided filter expression in the source stream.
    /// </exception>
    public bool Where(Func<TEarlier, bool> filterExpression)
    {
        int indexOfLaterEvent = sourceStream.ToList().IndexOf(laterEvent);

        TEarlier? earlierEvent = sourceStream.OfType<TEarlier>().FirstOrDefault(filterExpression);

        if (earlierEvent is null)
        {
            throw new ArgumentException($"Can't find Event of type {typeof(TEarlier)} that fits to given filter expression.");
        }

        int indexOfEarlierEvent = sourceStream.ToList().IndexOf(earlierEvent);

        return indexOfEarlierEvent < indexOfLaterEvent;
    }
}
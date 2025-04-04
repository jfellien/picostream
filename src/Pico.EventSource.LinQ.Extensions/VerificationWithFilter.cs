using Pico.EventSource.Models;

namespace Pico.EventSource.LinQ.Extensions;

/// <summary>
/// Represents a domain event query system with an applied filter condition, allowing checks and queries
/// based on a filtered subset of domain events from a given source stream.
/// </summary>
/// <typeparam name="TSource">
/// The type of the domain event being queried, which must implement <see cref="IDomainEvent"/>.
/// </typeparam>
public class VerificationWithFilter<TSource>(
    IEnumerable<object> sourceStream,
    Func<TSource, bool> filterExpression) where TSource : IDomainEvent
{
    /// <summary>
    /// Determines if domain events of type <typeparamref name="TEarlierEvent"/> occurred earlier than
    /// the event of type <typeparamref name="TSource"/> that meets the provided filter condition within the source stream.
    /// </summary>
    /// <typeparam name="TEarlierEvent">
    /// The type of the domain event to check against, which must implement <see cref="IDomainEvent"/>.
    /// </typeparam>
    /// <returns>
    /// A <see cref="HappenedLaterVerification{TEarlier,TLater}"/> instance that allows further
    /// evaluation to check whether <typeparamref name="TEarlierEvent"/> occurred before the current filtered <typeparamref name="TSource"/> event.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if no events of type <typeparamref name="TSource"/> exist in the source stream, or if no event
    /// satisfies the filter condition.
    /// </exception>
    public HappenedLaterVerification<TEarlierEvent, TSource> HappenedLaterThan<TEarlierEvent>()
        where TEarlierEvent : IDomainEvent
    {
        if (sourceStream.OfType<TSource>().Any() == false)
        {
            throw new ArgumentException(
                $"This type of event ({typeof(TSource).Name}) is not part of current events sequence.");
        }

        TSource? laterEvent = sourceStream.OfType<TSource>().LastOrDefault(filterExpression);

        if (laterEvent is null)
        {
            throw new ArgumentException(
                $"Can't find Event of type {typeof(TSource)} that fits to given filter expression.");
        }

        return new HappenedLaterVerification<TEarlierEvent, TSource>(sourceStream, laterEvent);
    }

    /// <summary>
    /// Determines if domain events of type <typeparamref name="TLaterEvent"/> occurred later than
    /// the event of type <typeparamref name="TSource"/> that meets the provided filter condition within the source stream.
    /// </summary>
    /// <typeparam name="TLaterEvent">
    /// The type of the domain event to check against, which must implement <see cref="IDomainEvent"/>.
    /// </typeparam>
    /// <returns>
    /// A <see cref="HappenedEarlierVerification{TEarlier,TLater}"/> instance that allows further
    /// evaluation to check whether <typeparamref name="TLaterEvent"/> occurred after the current filtered <typeparamref name="TSource"/> event.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if no events of type <typeparamref name="TSource"/> exist in the source stream, or if no event
    /// satisfies the filter condition.
    /// </exception>
    public HappenedEarlierVerification<TSource, TLaterEvent> HappenedEarlierThan<TLaterEvent>()
        where TLaterEvent : IDomainEvent
    {
        if (sourceStream.OfType<TSource>().Any() == false)
        {
            throw new ArgumentException(
                $"This type of event ({typeof(TSource).Name}) is not part of current events sequence.");
        }

        TSource? earlierEvent = sourceStream.OfType<TSource>().FirstOrDefault(filterExpression);

        if (earlierEvent == null)
        {
            throw new ArgumentException(
                $"Can't find Event of type {typeof(TSource)} that fits to given filter expression.");
        }

        return new HappenedEarlierVerification<TSource, TLaterEvent>(sourceStream, earlierEvent);
    }

    public bool Exists()
    {
        return sourceStream.OfType<TSource>().Any(filterExpression);
    }
}
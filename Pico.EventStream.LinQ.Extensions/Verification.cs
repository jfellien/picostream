using Pico.EventStream.Models;

namespace Pico.EventStream.LinQ.Extensions;

/// <summary>
/// Represents a set of checks that can be performed on a stream of domain events
/// to determine the occurrence and order of specific events.
/// </summary>
/// <typeparam name="TSource">
/// The type of domain event to check for in the event stream. It must implement <see cref="IDomainEvent"/>.
/// </typeparam>
public class Verification<TSource>(IEnumerable<object> sourceStream) where TSource : IDomainEvent
{
    /// <summary>
    /// Filters the stream of domain events to include only those matching the specified condition.
    /// </summary>
    /// <param name="filterExpression">
    /// A function that defines the condition to filter the domain events. It takes an event of type <typeparamref name="TSource"/>
    /// and returns a boolean indicating whether the event matches the condition.
    /// </param>
    /// <returns>
    /// An instance of <see cref="VerificationWithFilter{TSource}"/> that includes the filtered stream of events
    /// and allows additional checks or queries on the filtered events.
    /// </returns>
    public VerificationWithFilter<TSource> Where(Func<TSource, bool> filterExpression)
    {
        return new VerificationWithFilter<TSource>(sourceStream, filterExpression);
    }

    /// <summary>
    /// Determines whether any event of type <typeparamref name="TSource"/> in the event stream
    /// occurred earlier than any event of type <typeparamref name="TComparer"/>.
    /// </summary>
    /// <typeparam name="TComparer">
    /// The type of domain event to compare against the <typeparamref name="TSource"/> events in the stream.
    /// </typeparam>
    /// <returns>
    /// A boolean value indicating whether a <typeparamref name="TSource"/> event
    /// occurred earlier in the stream than a <typeparamref name="TComparer"/> event.
    /// Returns true if such an order is found; otherwise, false.
    /// </returns>
    public bool HappenedEarlierThan<TComparer>()
    {
        bool sourceEventFound = false;
        bool compareEventFound = false;

        foreach (object domainEvent in sourceStream)
        {
            if (domainEvent is TSource)
            {
                sourceEventFound = true;
            }

            if (domainEvent is TComparer)
            {
                compareEventFound = true;
            }

            if (sourceEventFound && compareEventFound == false)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether an event of type <typeparamref name="TSource"/> occurred later than an event of type <typeparamref name="TComparer"/>
    /// in the sequence of domain events.
    /// </summary>
    /// <typeparam name="TComparer">
    /// The type of the domain event to compare against. It must implement <see cref="IDomainEvent"/>.
    /// </typeparam>
    /// <returns>
    /// A boolean value indicating whether an event of type <typeparamref name="TSource"/> occurred later in the sequence
    /// than an event of type <typeparamref name="TComparer"/>. Returns true if a <typeparamref name="TSource"/> event occurred later,
    /// otherwise false.
    /// </returns>
    public bool HappenedLaterThan<TComparer>()
    {
        bool sourceEventFound = false;
        bool comparsionEventFound = false;

        foreach (object domainEvent in sourceStream.Reverse())
        {
            if (domainEvent is TSource)
            {
                sourceEventFound = true;
            }

            if (domainEvent is TComparer)
            {
                comparsionEventFound = true;
            }

            if (sourceEventFound && comparsionEventFound == false)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether any domain events of the specified type exist in the event stream.
    /// </summary>
    /// <returns>
    /// True if at least one domain event of type <typeparamref name="TSource"/> is present in the stream; otherwise, false.
    /// </returns>
    public bool Exists()
    {
        return sourceStream.OfType<TSource>().Any();
    }
}
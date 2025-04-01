using Pico.EventStore.Models;
using Pico.EventStore.Persistence;

namespace Pico.EventStore;

/// <summary>
/// Represents a stream of domain events specific to a context, entity, and entity ID,
/// with functionality to append and retrieve events in an organized manner.
/// </summary>
public class DomainEvents
{
    private readonly string _context;
    private readonly string _entity;
    private readonly string _entityId;
    private readonly IReadAndWriteDomainEvents _storage;

    private DomainEventSequence _historySequence;

    /// <summary>
    /// Represents a stream of domain events associated with a specific context, entity, and entity ID.
    /// </summary>
    public DomainEvents(string context, string entity, string entityId, IReadAndWriteDomainEvents storage)
    {
        _context = context;
        _entity = entity;
        _entityId = entityId;
        _storage = storage;

        _historySequence = [];
    }

    /// <summary>
    /// Adds a single domain event to the current stream and persists it to storage.
    /// </summary>
    /// <param name="domainEvent">The domain event to be appended to the stream.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    public Task Append(IDomainEvent domainEvent)
    {
        return Append(new List<IDomainEvent>
        {
            domainEvent
        });
    }

    /// <summary>
    /// Appends a single domain event to the current event stream while associating it with a specific entity ID.
    /// </summary>
    /// <param name="domainEvent">The domain event to append.</param>
    /// <param name="entityId">The unique identifier of the entity to associate with the domain event.</param>
    /// <returns>A task that represents the asynchronous operation of appending the domain event.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided entityId is null or empty.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided entityId is different from the entityId already associated
    /// with this instance of the domain events stream.
    /// </exception>
    public Task Append(IDomainEvent domainEvent, string entityId)
    {
        return Append(new List<IDomainEvent>
        {
            domainEvent
        }, entityId);
    }

    /// <summary>
    /// Appends a collection of domain events to the event stream and persists them in storage.
    /// </summary>
    /// <param name="domainEvents">The collection of domain events to append to the stream.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    public Task Append(IEnumerable<IDomainEvent> domainEvents)
    {
        return WriteToStorageAndLocalHistory(domainEvents, _context, _entity, _entityId);
    }

    /// <summary>
    /// Appends a collection of domain events to the current stream while associating them with a specific entity ID.
    /// </summary>
    /// <param name="domainEvents">A collection of domain events to append.</param>
    /// <param name="entityId">The unique identifier of the entity associated with the events.</param>
    /// <returns>A task that represents the asynchronous operation of appending the domain events.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided entityId is null or empty.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided entityId is different from the entityId already associated
    /// with this instance of the domain events stream.
    /// </exception>
    public Task Append(IEnumerable<IDomainEvent> domainEvents, string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            throw new ArgumentNullException(entityId);
        }

        if (string.IsNullOrWhiteSpace(_entityId) == false
            && string.IsNullOrWhiteSpace(entityId) == false
            && string.Equals(_entityId, entityId, StringComparison.InvariantCultureIgnoreCase) == false)
        {
            throw new ArgumentException(
                "You have set entityId but this instance of EventStream already have, but different, an EntityId");
        }

        return WriteToStorageAndLocalHistory(domainEvents, _context, _entity, entityId);
    }

    /// <summary>
    /// Retrieves the current stream of domain events, combining stored events and any newly appended in-memory events if present.
    /// </summary>
    /// <returns>An asynchronous task that resolves to a collection of domain events.</returns>
    public async Task<IEnumerable<IDomainEvent>> Events()
    {
        if (_historySequence.HasBeenSequenced)
        {
            return _historySequence.Select(x => x.Instance);
        }

        DomainEventSequence storedSequence = await GetFromStorageByGivenParameters();

        if (_historySequence.Any())
        {
            List<SequencedDomainEvent> currentSequence = _historySequence.ToList();
            _historySequence.Clear();

            _historySequence.AddRange(storedSequence);
            _historySequence.AddRange(currentSequence);
        }
        else
        {
            _historySequence = storedSequence;
        }

        return _historySequence.Select(x => x.Instance);
    }

    /// <summary>
    /// Retrieves a sequence of domain events from storage based on the currently specified context, entity, and entity ID parameters.
    /// Determines the appropriate retrieval method based on the presence or absence of the entity and entity ID values.
    /// </summary>
    /// <returns>A <see cref="DomainEventSequence"/> representing the retrieved sequence of domain events, or an empty sequence if no events are found.</returns>
    private async Task<DomainEventSequence> GetFromStorageByGivenParameters()
    {
        DomainEventSequence? domainEventSequence = null;

        if (string.IsNullOrWhiteSpace(_entity)
            && string.IsNullOrWhiteSpace(_entityId))
        {
            domainEventSequence = await _storage.ReadBy(_context, CancellationToken.None);
        }

        if (string.IsNullOrWhiteSpace(_entity) == false
            && string.IsNullOrWhiteSpace(_entityId))
        {
            domainEventSequence = await _storage.ReadBy(_context, _entity, CancellationToken.None);
        }

        if (string.IsNullOrWhiteSpace(_entity) == false
            && string.IsNullOrWhiteSpace(_entityId) == false)
        {
            domainEventSequence = await _storage.ReadBy(_context, _entity, _entityId, CancellationToken.None);
        }

        return domainEventSequence ?? [];
    }

    /// <summary>
    /// Appends a collection of domain events to the storage and local history for a specified context, entity, and entity ID.
    /// </summary>
    /// <param name="domainEvents">The collection of domain events to be written.</param>
    /// <param name="context">The context associated with the domain events.</param>
    /// <param name="entity">The entity associated with the domain events.</param>
    /// <param name="entityId">The unique identifier of the entity associated with the domain events.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task WriteToStorageAndLocalHistory(
        IEnumerable<IDomainEvent> domainEvents,
        string context,
        string entity,
        string entityId)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            long sequenceNumber = await WriteToStorage(domainEvent, context, entity, entityId);

            AddToLocalHistory(domainEvent, sequenceNumber);
        }
    }

    /// <summary>
    /// Writes a single domain event to the persistent storage for a specified context, entity, and entity ID.
    /// </summary>
    /// <param name="domainEvent">The domain event to persist to storage.</param>
    /// <param name="context">The context associated with the domain event used for categorization.</param>
    /// <param name="entity">The name of the entity related to the domain event.</param>
    /// <param name="entityId">The ID of the specific entity instance.</param>
    /// <returns>A task representing the asynchronous operation, containing the sequence number of the written event.</returns>
    private async Task<long> WriteToStorage(IDomainEvent domainEvent, string context, string entity, string entityId)
    {
        return await _storage.Write(domainEvent, context, entity, entityId);
    }

    /// <summary>
    /// Adds a domain event to the local in-memory history and associates it with a specified sequence number.
    /// </summary>
    /// <param name="domainEvent">The domain event to add to the local history.</param>
    /// <param name="sequenceNumber">The sequence number associated with the domain event.</param>
    private void AddToLocalHistory(IDomainEvent domainEvent, long sequenceNumber)
    {
        _historySequence.Add(new SequencedDomainEvent(sequenceNumber, domainEvent));
    }
}
namespace Pico.EventStream.Persistence;

public interface IReadAndWriteDomainEvents
{
    Task<DomainEventSequence> ReadBy(string context, CancellationToken cancellationToken = default);
    Task<DomainEventSequence> ReadBy(string context, string entity, CancellationToken cancellationToken = default);
    Task<DomainEventSequence> ReadBy(string context, string entity, string entityId, CancellationToken cancellationToken = default);
    Task<long> Write(IDomainEvent domainEvent, string context, string? entity = null, string? entityId = null, CancellationToken cancellationToken = default);
}
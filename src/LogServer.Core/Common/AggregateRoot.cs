using LogServer.Core.DomainEvents;

namespace LogServer.Core.Common;

/// <summary>
/// Base type for aggregate roots. Records domain events that should be
/// published after the aggregate is persisted.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void RaiseDomainEvent(DomainEvent @event) => _domainEvents.Add(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

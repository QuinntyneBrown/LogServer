using MediatR;

namespace LogServer.Core.DomainEvents;

/// <summary>
/// Marker base type for domain events. Implements <see cref="INotification"/>
/// so handlers can be wired through MediatR.
/// </summary>
public abstract class DomainEvent : INotification
{
    public Guid CorrelationId { get; set; }
    public Guid CausationId { get; set; }
    public Guid ActivityId { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}

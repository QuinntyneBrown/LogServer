using MediatR;
using System.Collections.Generic;

namespace LogServer.API
{
    public interface IEntity
    {
        IReadOnlyCollection<INotification> DomainEvents { get; }
        void RaiseDomainEvent(INotification eventItem);
        void ClearEvents();
    }
}

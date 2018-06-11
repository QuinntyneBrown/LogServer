using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LogServer.API
{
    public class Log
    {
        public Log()
        {
            _domainEvents = new List<INotification>();
        }
        [Key]
        public int LogId { get; set; }
        [Required, StringLength(50)]
        public string LogLevel { get; set; }
        [Required]
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
        public void RaiseDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}

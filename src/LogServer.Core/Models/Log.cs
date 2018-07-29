using LogServer.Core.Common;
using LogServer.Core.DomainEvents;
using System;

namespace LogServer.Core.Models
{
    public class Log: AggregateRoot
    {
        public Log(string logLevel, string message, Guid clientId)
            => Apply(new LogCreated(logLevel,message,clientId,LogId));

        public Guid LogId { get; set; } = Guid.NewGuid();
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; }
        protected override void EnsureValidState()
        {
            if (string.IsNullOrEmpty(Message))
                throw new Exception("Invalid Aggregate");

            if (default(Guid) == ClientId)
                throw new Exception("Invalid Aggregate");
        }

        protected override void When(DomainEvent @event)
        {
            switch (@event)
            {
                case LogCreated logCreated:                    
					LogId = logCreated.LogId;
                    LogLevel = logCreated.LogLevel;
                    Message = logCreated.Message;
                    ClientId = logCreated.ClientId;
                    CreatedOn = logCreated.CreatedOn;
                    break;
            }
        }
    }
}

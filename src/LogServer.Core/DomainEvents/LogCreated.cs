using System;

namespace LogServer.Core.DomainEvents
{
    public class LogCreated: DomainEvent
    {
        public LogCreated(string logLevel,string message,Guid clientId, Guid logId)
        {
            LogLevel = logLevel;
            Message = message;
            ClientId = clientId;
            LogId = logId;
        }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public Guid LogId { get; set; }
    }
}

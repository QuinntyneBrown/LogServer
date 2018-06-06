using System;

namespace LogServer.API
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

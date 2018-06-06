using System;

namespace LogService
{
    public class Log
    {
        public int LogId { get; set; }
        public int LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

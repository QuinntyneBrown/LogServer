using System;

namespace LogServer.API
{
    public class LogApiModel
    {        
        public int LogId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public static LogApiModel FromLog(Log log)
            => new LogApiModel
            {
                LogId = log.LogId,
                Message = log.Message,
                LogLevel = log.LogLevel,
                ClientId = log.ClientId,
                CreatedOn = log.CreatedOn
            };
    }
}

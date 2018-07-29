using LogServer.Core.Models;
using System;

namespace LogServer.API
{
    public class LogDto
    {        
        public Guid LogId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public string CreatedOn { get; set; }
        public static LogDto FromLog(Log log)
            => new LogDto
            {
                LogId = log.LogId,
                LogLevel = log.LogLevel,
                Message = log.Message,
                ClientId = log.ClientId,
                CreatedOn = $"{log.CreatedOn.ToShortDateString()} {log.CreatedOn.ToLongTimeString()}"
            };
    }
}

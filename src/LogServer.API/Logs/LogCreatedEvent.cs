using MediatR;

namespace LogServer.API.Logs
{
    public class LogCreatedEvent: INotification
    {
        public LogCreatedEvent(Log log)
        {
            Payload = new { log };
        }

        public string Type { get; set; } = "[Log] Created";
        public dynamic Payload { get; set; }
    }
}

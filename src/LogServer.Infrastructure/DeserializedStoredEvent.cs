using LogServer.Core.Models;
using System;
using static Newtonsoft.Json.JsonConvert;

namespace LogServer.Infrastructure
{
    public class DeserializedStoredEvent {
        public DeserializedStoredEvent(StoredEvent @event)
        {            
            StoredEventId = @event.StoredEventId;
            StreamId = @event.StreamId;
            Type = @event.Type;
            Aggregate = @event.Aggregate;
            Data = DeserializeObject(@event.Data, System.Type.GetType(@event.DotNetType));
            DotNetType = @event.DotNetType;
            CreatedOn = @event.CreatedOn;
            Version = @event.Version;
        }

        public Guid StoredEventId { get; set; }
        public Guid StreamId { get; set; }
        public string Type { get; set; }
        public string Aggregate { get; set; }
        public object Data { get; set; }
        public string DotNetType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Version { get; set; }
    }
}

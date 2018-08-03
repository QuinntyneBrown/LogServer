using LogServer.Core.Common;
using System;
using static Newtonsoft.Json.JsonConvert;

namespace LogServer.Core.Models
{
    public class StoredEvent
    {
        public StoredEvent()
        {

        }
        public StoredEvent(AggregateRoot aggregateRoot, object @event,Type type)
        {
            StoredEventId = Guid.NewGuid();
            Aggregate = aggregateRoot.GetType().Name;
            Data = SerializeObject(@event);
            StreamId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregateRoot, null);
            DotNetType = @event.GetType().AssemblyQualifiedName;
            Type = @event.GetType().Name;
            CreatedOn = DateTime.UtcNow;
        }

        public Guid StoredEventId { get; set; }
        public Guid StreamId { get; set; }
        public string Type { get; set; }
        public string Aggregate { get; set; }
        public string Data { get; set; }
        public string DotNetType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Version { get; set; }
    }
}

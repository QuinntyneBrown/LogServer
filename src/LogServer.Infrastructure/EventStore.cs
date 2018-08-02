using LogServer.Core.Common;
using LogServer.Core.DomainEvents;
using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;


namespace LogServer.Infrastructure
{
    public static class DeserializedEventStore
    {
        public static ConcurrentDictionary<Guid, DeserializedStoredEvent> Events { get; set; }
        private static readonly object syncLock = new object();

        public static void TryAdd(StoredEvent @event)
        {
            if (Events == null)
                Events = new ConcurrentDictionary<Guid, DeserializedStoredEvent>(GetStoredEvents().Select(x => new DeserializedStoredEvent(x)).ToDictionary(x => x.StoredEventId));

            Events.TryAdd(@event.StoredEventId, new DeserializedStoredEvent(@event));
        }

        public static List<DeserializedStoredEvent> GetDeserializedStoredEvents() {

            if (Events == null)
                Events = new ConcurrentDictionary<Guid, DeserializedStoredEvent>(GetStoredEvents().Select(x => new DeserializedStoredEvent(x)).ToDictionary(x => x.StoredEventId));

            return Events.Select(x => x.Value)
                .OrderBy(x => x.CreatedOn)
                .ToList();
        }

        public static ICollection<StoredEvent> GetStoredEvents()
            => DeserializeObject<ICollection<StoredEvent>>(string.Join(" ", File.ReadAllLines($@"{Environment.CurrentDirectory}\storedEvents.json")));

    }

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

    public class EventStore : IEventStore
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _queue;

        public EventStore(
            IMediator mediator = default(IMediator), 
            IBackgroundTaskQueue queue = default(IBackgroundTaskQueue)) {
            _mediator = mediator;
            _queue = queue;
        }

        public void Dispose() { }

        public void Save(AggregateRoot aggregateRoot)
        {
            var type = aggregateRoot.GetType();

            foreach (var @event in aggregateRoot.DomainEvents)
            {
                @event.CreatedOn = DateTime.UtcNow;

                Add(new StoredEvent()
                {
                    StoredEventId = Guid.NewGuid(),
                    Aggregate = aggregateRoot.GetType().Name,
                    Data = SerializeObject(@event),
                    StreamId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregateRoot, null),
                    DotNetType = @event.GetType().AssemblyQualifiedName,
                    Type = @event.GetType().Name,
                    CreatedOn = DateTime.UtcNow
                });
                
                if (_mediator != null) _mediator.Publish(@event).GetAwaiter().GetResult();
            }

            aggregateRoot.ClearEvents();
        }

        public T Query<T>(Guid id)
            where T : AggregateRoot
        {
            var list = new List<DomainEvent>();

            foreach (var storedEvent in Get().Where(x => x.StreamId == id))
                list.Add(storedEvent.Data as DomainEvent);

            return Load<T>(list.ToArray());
        }

        private T Load<T>(DomainEvent[] events)
            where T : AggregateRoot
        {
            var aggregate = (T)FormatterServices.GetUninitializedObject(typeof(T));

            foreach (var @event in events) aggregate.Apply(@event);

            aggregate.ClearEvents();

            return aggregate;
        }

        public TAggregateRoot Query<TAggregateRoot>(string propertyName, string value)
            where TAggregateRoot : AggregateRoot
        {
            var storedEvents = Get()
                .Where(x => {
                    var prop = Type.GetType(x.DotNetType).GetProperty(propertyName);
                    return prop != null && $"{prop.GetValue(x.Data, null)}" == value;
                })
                .ToArray();

            if (storedEvents.Length < 1) return null;

            return Query<TAggregateRoot>(storedEvents.First().StreamId) as TAggregateRoot;
        }

        public TAggregateRoot[] Query<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot
        {
            var aggregates = new List<TAggregateRoot>();
            
            foreach (var grouping in Get()
                .Where(x => x.Aggregate == typeof(TAggregateRoot).Name).GroupBy(x => x.StreamId))
            {                
                var events = grouping.Select(x => x.Data as DomainEvent).ToArray();
                
                aggregates.Add(Load<TAggregateRoot>(events.ToArray()));
            }
            
            return aggregates.ToArray();
        }  
        
        protected List<DeserializedStoredEvent> Get() {
            return DeserializedEventStore.GetDeserializedStoredEvents();
        }

        protected void Add(StoredEvent @event) {
            DeserializedEventStore.TryAdd(@event);
            Persist(@event);
        }
        
        public void Persist(StoredEvent @event)
            => _queue.QueueBackgroundWorkItem(async token =>
            {
                var storedEvents = DeserializedEventStore.GetStoredEvents();
                storedEvents.Add(@event);
                File.WriteAllLines($@"{Environment.CurrentDirectory}\storedEvents.json", 
                    new string[1] { SerializeObject(storedEvents) });
                await Task.CompletedTask;
            });
    }
}

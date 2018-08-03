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
using static LogServer.Infrastructure.DeserializedEventStore;

namespace LogServer.Infrastructure
{
    internal static class DeserializedEventStore
    {
        private static ConcurrentDictionary<Guid, DeserializedStoredEvent> _events { get; set; }
        public static ConcurrentDictionary<Guid, DeserializedStoredEvent> Events
        {
            get {
                if(_events == null)
                {
                    var dictionary = new Dictionary<Guid, DeserializedStoredEvent>();

                    foreach (var storedEvent in GetStoredEvents())
                        dictionary.Add(storedEvent.StreamId, new DeserializedStoredEvent(storedEvent));

                    _events = new ConcurrentDictionary<Guid, DeserializedStoredEvent>(dictionary);
                }
                
                return _events;
            }
        }

        private static readonly object syncLock = new object();

        public static void TryAdd(StoredEvent @event)
            => Events.TryAdd(@event.StoredEventId, new DeserializedStoredEvent(@event));

        public static IEnumerable<DeserializedStoredEvent> Get()
        {
            var eventsCount = Events.Count();
            var deserializedStoredEvents = new DeserializedStoredEvent[eventsCount];
            for (var i = 0; i < eventsCount; i++)
                deserializedStoredEvents[i] = Events.ElementAt(i).Value;

            Array.Sort(deserializedStoredEvents, (x, y) => DateTime.Compare(x.CreatedOn, y.CreatedOn));
            return deserializedStoredEvents;
        }

        public static IEnumerable<StoredEvent> GetStoredEvents()
            => DeserializeObject<ICollection<StoredEvent>>(string.Join(" ",File.ReadAllLines($@"{Environment.CurrentDirectory}\storedEvents.json")));

    }

    internal class DeserializedStoredEvent {
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

                Add(new StoredEvent(aggregateRoot,@event,type));
                
                if (_mediator != null) _mediator.Publish(@event).GetAwaiter().GetResult();
            }

            aggregateRoot.ClearEvents();
        }

        public T Query<T>(Guid id)
            where T : AggregateRoot
        {
            var list = new List<DomainEvent>();

            foreach (var storedEvent in Get()) {
                if(storedEvent.StreamId == id)
                    list.Add(storedEvent.Data as DomainEvent);
            }
            
            return Load<T>(list);
        }
        
        private T Load<T>(IEnumerable<DomainEvent> events)
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
            var type = typeof(TAggregateRoot);
            var prop = type.GetProperty(propertyName);

            var storedEvents = Get()
                .Where(x => prop != null && $"{prop.GetValue(x.Data, null)}" == value);

            if (storedEvents.Count() < 1) return null;

            return Query<TAggregateRoot>(storedEvents.ElementAt(0).StreamId) as TAggregateRoot;
        }


        public IEnumerable<TAggregateRoot> Query<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot
        {
            var aggregates = new List<TAggregateRoot>();            
            var streamIds = new List<Guid>();

            foreach(var @event in Get())
                if (!streamIds.Contains(@event.StreamId))
                    streamIds.Add(@event.StreamId);

            foreach(var streamId in streamIds)
                aggregates.Add(Query<TAggregateRoot>(streamId));
            
            return aggregates;
        }
        
        private void Add(StoredEvent @event) {
            TryAdd(@event);
            Persist(@event);
        }
        
        private void Persist(StoredEvent @event)
            => _queue.QueueBackgroundWorkItem(async token =>
            {
                var payload = SerializeObject(DeserializedEventStore
                    .GetStoredEvents().Concat(new StoredEvent[1] { @event }));

                File.WriteAllText($@"{Environment.CurrentDirectory}\storedEvents.json", payload);
                await Task.CompletedTask;
            });
    }
}

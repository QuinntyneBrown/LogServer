using LogServer.Core.Common;
using LogServer.Core.DomainEvents;
using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace LogServer.Infrastructure
{
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
                
                _mediator?.Publish(@event).GetAwaiter().GetResult();
            }

            aggregateRoot.ClearEvents();
        }

        public T Query<T>(Guid id)
            where T : AggregateRoot
        {
            var list = new List<DomainEvent>();

            foreach (var storedEvent in DeserializedEventStore.Get()) 
                if(storedEvent.StreamId == id)
                    list.Add(storedEvent.Data as DomainEvent);
            
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

            if (prop == null) return null;

            foreach (var deserializedStoredEvent in DeserializedEventStore.Get())
                if ($"{prop.GetValue(deserializedStoredEvent.Data, null)}" == value)
                    return Query<TAggregateRoot>(deserializedStoredEvent.StreamId) as TAggregateRoot;
                        
            return null;
        }


        public IEnumerable<TAggregateRoot> Query<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot
        {
            var aggregates = new List<TAggregateRoot>();            
            var streamIds = new List<Guid>();
            var name = typeof(TAggregateRoot).Name;

            foreach(var @event in DeserializedEventStore.Get())
                if (@event.Aggregate == name && !streamIds.Contains(@event.StreamId))
                    streamIds.Add(@event.StreamId);

            foreach(var streamId in streamIds)
                aggregates.Add(Query<TAggregateRoot>(streamId));
            
            return aggregates;
        }

        private void Add(StoredEvent @event)
        {            
            DeserializedEventStore.TryAdd(@event);

            _queue.QueueBackgroundWorkItem(async token =>
            {                
                var payload = SerializeObject(DeserializedEventStore
                    .GetStoredEvents().Concat(new StoredEvent[1] { @event }));

                File.WriteAllText($@"{Environment.CurrentDirectory}\storedEvents.json", payload);
                await Task.CompletedTask;
            });
        }            
    }
}

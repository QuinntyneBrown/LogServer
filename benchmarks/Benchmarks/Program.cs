using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using LogServer.Infrastructure;
using System;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<EventStoreBenchMarks>();
        }

    }

    public class EventStoreBenchMarks {
        private readonly IEventStore _eventStore;

        public EventStoreBenchMarks()
        {
            _eventStore = new EventStore(null, new BackgroundTaskQueue());
        }


        [Benchmark]
        public void Something() {
            _eventStore.Save(new Log("Debug", "Debug", Guid.NewGuid()));
        }
    }
}
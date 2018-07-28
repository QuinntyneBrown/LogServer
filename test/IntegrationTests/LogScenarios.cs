using LogServer.Core.Models;
using LogServer.Core.Extensions;
using LogServer.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LogServer.API;
using LogServer.Infrastructure;
using MediatR;
using LogServer.Infrastructure;
using System.Collections.Generic;
using System.Threading;

namespace IntegrationTests
{
    public class LogScenarios: LogScenarioBase
    {

        [Fact]
        public async Task ShouldSave()
        {
            using (var server = CreateServer())
            {
                IMediator mediator = server.Host.Services.GetService(typeof(IMediator)) as IMediator;
                IBackgroundTaskQueue queue = server.Host.Services.GetService(typeof(IBackgroundTaskQueue)) as IBackgroundTaskQueue;

                var eventStore = new EventStore(mediator,queue);
                var id = Guid.NewGuid();

                var response = await server.CreateClient()
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = id,
                        LogLevel = "Trace",
                        Message = "Q"
                    });

                var aggregate = eventStore.Query<Log>(response.LogId);

                Assert.Equal("Q", aggregate.Message);
            }
        }

        [Fact]
        public async Task ShouldSaveMultiple()
        {
            using (var server = CreateServer())
            {
                IMediator mediator = server.Host.Services.GetService(typeof(IMediator)) as IMediator;
                IBackgroundTaskQueue queue = server.Host.Services.GetService(typeof(IBackgroundTaskQueue)) as IBackgroundTaskQueue;

                var eventStore = new EventStore(mediator, queue);
                var id = Guid.NewGuid();
                var client = server.CreateClient();

                var taskList = new List<Task>
                {
                    client
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = id,
                        LogLevel = "Trace",
                        Message = "1"
                    }),

                    client
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = id,
                        LogLevel = "Trace",
                        Message = "2"
                    }),

                    client
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = id,
                        LogLevel = "Trace",
                        Message = "3"
                    })
                };

                await Task.WhenAll(taskList);
                
                Assert.Equal(1, 1);
            }
        }

        [Fact]
        public async Task ShouldGetAll()
        {
            using (var server = CreateServer())
            {
                _ = await server.CreateClient()
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = Guid.NewGuid(),
                        LogLevel = "Trace",
                        Message = "Test"
                    });

                var response = await server.CreateClient()
                    .GetAsync<GetLogsQuery.Response>(Get.Logs);

                Assert.True(response.Logs.Count() > 0);
            }
        }

        [Fact]
        public async Task ShouldGetById()
        {
            using (var server = CreateServer())
            {
                var  response1 = await server.CreateClient()
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = Guid.NewGuid(),
                        LogLevel = "Trace",
                        Message = "Test"
                    });

                var response2 = await server.CreateClient()
                    .GetAsync<GetLogByIdQuery.Response>(Get.LogById(response1.LogId));

                Assert.Equal(response2.Log.LogId,response1.LogId);
            }
        }
    }
}

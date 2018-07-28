using LogServer.Core.Models;
using LogServer.Core.Extensions;
using LogServer.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LogServer.API;
using LogServer.Infrastructure.Data;
using MediatR;

namespace IntegrationTests
{
    public class LogScenarios: LogScenarioBase
    {

        [Fact]
        public async Task ShouldSave()
        {
            using (var server = CreateServer())
            {
                IAppDbContext context = server.Host.Services.GetService(typeof(IAppDbContext)) as IAppDbContext;
                IMediator mediator = server.Host.Services.GetService(typeof(IMediator)) as IMediator;

                var eventStore = new EventStore(context, mediator);
                var id = Guid.NewGuid();

                var response = await server.CreateClient()
                    .PostAsAsync<CreateLogCommand.Request, CreateLogCommand.Response>(Post.Logs, new CreateLogCommand.Request()
                    {
                        ClientId = id,
                        LogLevel = "Trace",
                        Message = "Test"
                    });

                var aggregate = eventStore.Query<Log>(response.LogId);

                Assert.Equal("Test", aggregate.Message);
            }
        }

        [Fact]
        public async Task ShouldGetAll()
        {
            using (var server = CreateServer())
            {
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
                var response = await server.CreateClient()
                    .GetAsync<GetLogsQuery.Response>(Get.Logs);

                Assert.True(response.Logs.Count() > 0);
            }
        }
    }
}

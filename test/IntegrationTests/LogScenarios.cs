using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LogServer.Application.Logs.Commands.CreateLog;
using LogServer.Application.Logs.Queries.GetLogById;
using LogServer.Application.Logs.Queries.GetLogs;
using Xunit;

namespace IntegrationTests;

public class LogScenarios : IClassFixture<LogServerWebApplicationFactory>
{
    private readonly LogServerWebApplicationFactory _factory;

    public LogScenarios(LogServerWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task ShouldCreateLog()
    {
        var client = _factory.CreateClient();
        var clientId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync("api/logs", new CreateLogCommand
        {
            ClientId = clientId,
            LogLevel = "Trace",
            Message = "Q",
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CreateLogResponse>();
        body!.LogId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldGetById()
    {
        var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("api/logs", new CreateLogCommand
        {
            ClientId = Guid.NewGuid(),
            LogLevel = "Trace",
            Message = "Test",
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CreateLogResponse>();

        var getResponse = await client.GetFromJsonAsync<GetLogByIdResponse>($"api/logs/{created!.LogId}");

        getResponse!.Log.Should().NotBeNull();
        getResponse.Log!.LogId.Should().Be(created.LogId);
        getResponse.Log.Message.Should().Be("Test");
    }

    [Fact]
    public async Task ShouldGetAll()
    {
        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("api/logs", new CreateLogCommand
        {
            ClientId = Guid.NewGuid(),
            LogLevel = "Trace",
            Message = "Test",
        });

        var response = await client.GetFromJsonAsync<GetLogsResponse>("api/logs");

        response!.Logs.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenRequestIsInvalid()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/logs", new CreateLogCommand
        {
            ClientId = Guid.Empty,
            LogLevel = "",
            Message = "",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenLogDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"api/logs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

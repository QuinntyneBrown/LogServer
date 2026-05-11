using LogServer.Application.Logs.Commands.CreateLog;
using LogServer.Application.Logs.Queries.GetLogById;
using LogServer.Application.Logs.Queries.GetLogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogServer.API.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<CreateLogResponse>> Create(
        [FromBody] CreateLogCommand command,
        CancellationToken cancellationToken)
        => await _mediator.Send(command, cancellationToken);

    [HttpGet]
    public async Task<ActionResult<GetLogsResponse>> Get(CancellationToken cancellationToken)
        => await _mediator.Send(new GetLogsQuery(), cancellationToken);

    [HttpGet("{logId:guid}")]
    public async Task<ActionResult<GetLogByIdResponse>> GetById(
        Guid logId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetLogByIdQuery { LogId = logId }, cancellationToken);

        if (response.Log is null)
        {
            return NotFound();
        }

        return response;
    }
}

using LogServer.API.Logs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<CreateLogCommand.Response>> Create(CreateLogCommand.Request request)
            => await _mediator.Send(request);
        
        [HttpGet("{logId}")]
        public async Task<ActionResult<GetLogByIdQuery.Response>> GetById([FromRoute]GetLogByIdQuery.Request request)
            => await _mediator.Send(request);

        [HttpGet]
        public async Task<ActionResult<GetLogsQuery.Response>> Get()
            => await _mediator.Send(new GetLogsQuery.Request());
    }
}

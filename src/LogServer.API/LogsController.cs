using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<ActionResult<CreateLog.Response>> Create(CreateLog.Request request)
            => await _mediator.Send(request);

        [HttpGet("{logId}")]
        public async Task<ActionResult<GetLogById.Response>> GetById([FromRoute]GetLogById.Request request)
            => await _mediator.Send(request);

        [HttpGet]
        public async Task<ActionResult<GetLogs.Response>> Get()
            => await _mediator.Send(new GetLogs.Request());
        
    }
}
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using LogServer.Core.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using LogServer.Core.Models;

namespace LogServer.API
{
    public class CreateLogCommand
    {
        public class Request : IRequest<Response> {
            public string LogLevel { get; set; }
            public string Message { get; set; }
            public Guid ClientId { get; set; }
        }

        public class Response
        {

        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventStore _eventStore;

            public Handler(IEventStore eventStore) => _eventStore = eventStore;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                _eventStore.Save(new Log(request.LogLevel,request.Message,request.ClientId));

                return new Response() { };
            }
        }
    }
}

using LogServer.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LogServer.Application.Logs.Queries.GetLogById;

public sealed class GetLogByIdHandler : IRequestHandler<GetLogByIdQuery, GetLogByIdResponse>
{
    private readonly ILogServerDbContext _context;

    public GetLogByIdHandler(ILogServerDbContext context) => _context = context;

    public async Task<GetLogByIdResponse> Handle(GetLogByIdQuery request, CancellationToken cancellationToken)
    {
        var log = await _context.Logs
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LogId == request.LogId, cancellationToken);

        return new GetLogByIdResponse
        {
            Log = log is null ? null : LogDto.FromLog(log),
        };
    }
}

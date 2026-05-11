using LogServer.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LogServer.Application.Logs.Queries.GetLogs;

public sealed class GetLogsHandler : IRequestHandler<GetLogsQuery, GetLogsResponse>
{
    private readonly ILogServerDbContext _context;

    public GetLogsHandler(ILogServerDbContext context) => _context = context;

    public async Task<GetLogsResponse> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _context.Logs
            .AsNoTracking()
            .OrderBy(l => l.CreatedOn)
            .Select(l => LogDto.FromLog(l))
            .ToListAsync(cancellationToken);

        return new GetLogsResponse { Logs = logs };
    }
}

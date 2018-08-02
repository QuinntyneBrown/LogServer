using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LogServer.Core.Middleware
{
    public class RequestLoggerMiddleware
    {
        private const string _meessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _logger = logger;
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var start = Stopwatch.GetTimestamp();            
            try
            {
                await _next(httpContext);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
                var statusCode = httpContext.Response?.StatusCode;
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine("");
                }

                Console.WriteLine(string.Format("HTTP {0} {1} responded {2} in Elapsed:{3} ms", httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs));

                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine("");
                }
                _logger.LogTrace(_meessageTemplate, httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs);
            }
            catch
            {
                _logger.LogError(_meessageTemplate, httpContext.Request.Method, GetPath(httpContext), 500, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()));
            }
        }

        private double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        private string GetPath(HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();
        }
    }
}

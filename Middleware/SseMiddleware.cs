using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class SseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<string, HttpResponse> _subscribers = new ConcurrentDictionary<string, HttpResponse>();

    public SseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers["Accept"] == "text/event-stream")
        {
            var response = context.Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            var subscriberId = context.Request.Query["subscriberId"];

            if (!string.IsNullOrEmpty(subscriberId))
            {
                _subscribers.TryAdd(subscriberId, response);
            }

            while (!response.HttpContext.RequestAborted.IsCancellationRequested)
            {
                var message = "data: " + DateTime.Now.ToString("HH:mm:ss") + "\n\n";
                await response.WriteAsync(message);
                await response.Body.FlushAsync();
                await Task.Delay(1000);
            }
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}

public static class SseMiddlewareExtensions
{
    public static IApplicationBuilder UseSse(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SseMiddleware>();
    }
}

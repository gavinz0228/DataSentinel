using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
namespace DataSentinel.Infrastructure.Middlewares{

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger /* other dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
    {
        logger.LogError(ex, DateTime.Now.ToLongTimeString());
        context.Response.ContentType = "text/pain";
        return context.Response.WriteAsync(ex.Message);
    }
}

}
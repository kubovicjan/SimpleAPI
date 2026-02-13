namespace SimpleAPI.Core;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleAPI.Domain.Errors;

public class ExceptionFilter : IAsyncActionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        _logger.LogInformation($"Started processing request {context.HttpContext.Request.Path}...");

        var executedContext = await next().ConfigureAwait(false);
        var exception = executedContext.Exception;

        if (exception is not null)
        {
            var (statusCode, errorCode) = exception switch
            {
                InvalidDataException => (StatusCodes.Status400BadRequest, ErrorCodes.InvalidInputData),
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.Unknown)
            };

            _logger.LogError(exception, exception.Message);

            var response = new ErrorResponse
            {
                ErrorCode = (int)errorCode,
                ErrorMessage = exception.Message
            };

            executedContext.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            executedContext.ExceptionHandled = true;
        }

        _logger.LogInformation($"Finished processing request {context.HttpContext.Request.Path}...");
    }
}
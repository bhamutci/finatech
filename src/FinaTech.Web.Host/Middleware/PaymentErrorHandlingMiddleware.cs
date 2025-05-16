using System.Data.Common;
using System.Net.Mime;
using FinaTech.Application.Exceptions;

namespace FinaTech.Web.Host.Middleware;

/// <summary>
/// Middleware responsible for handling errors that occur during payment processing.
/// It captures exceptions thrown during the execution of the HTTP pipeline, logs the details of the error,
/// and ensures that appropriate responses are sent to the client.
/// </summary>
public class PaymentErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PaymentErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Middleware responsible for handling errors that occur during payment processing.
    /// Captures and logs exceptions, ensuring graceful handling of payment-related errors.
    /// </summary>
    public PaymentErrorHandlingMiddleware(RequestDelegate next, ILogger<PaymentErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middleware method responsible for processing HTTP requests and handling exceptions in the pipeline.
    /// Captures errors, logs them, and generates appropriate HTTP responses based on the exception type.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request and response.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline (including your endpoint)
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");

            context.Response.ContentType = MediaTypeNames.Application.Json; // Or ProblemDetails MIME type

            switch (ex)
            {
                case OperationCanceledException:
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                    await context.Response.WriteAsync("Request cancelled.");
                    break;

                case PaymentException paymentEx:
                    context.Response.StatusCode =
                        StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsJsonAsync(new
                        {ProblemDetails = paymentEx.Message});
                    break;

                case DbException dbEx:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new
                        {ProblemDetails = "A database error occurred."});
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request for invalid input
                    await context.Response.WriteAsJsonAsync(new {ProblemDetails = argEx.Message});
                    break;

                default:

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new
                        {ProblemDetails = "An unexpected internal server error occurred."});
                    break;
            }
        }
    }
}

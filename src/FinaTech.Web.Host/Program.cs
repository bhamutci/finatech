using System.Data.Common;
using FinaTech.Application;
using FinaTech.Application.Exceptions;
using FinaTech.Application.Services.Payment;
using FinaTech.Application.Services.Payment.Dto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration);
var app = builder.Build();

builder.Services.AddEndpointsApiExplorer();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoint for GET /payments/{id}
// Retrieves a single payment by ID
app.MapGet("/payments/{id:int}", async (int id, CancellationToken cancellationToken) =>
{
    try
    {
        IPaymentService paymentService = app.Services.GetService<IPaymentService>();
        var payment = await paymentService.GetPaymentAsync(id, cancellationToken);

        if (payment == null)
        {
            return Results.NotFound($"Payment with ID {id} not found.");
        }

        return Results.Ok(payment);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(499);
    }
    catch (PaymentException ex)
    {
        return Results.Json(new {Message= ex.Message }, statusCode:500);
    }
    catch (DbException ex)
    {
        return Results.Json(new {Message = ex.Message}, statusCode:500);
    }
    catch (Exception ex)
    {
        return Results.Json(new {Message = ex.Message}, statusCode:500);
    }
})
.WithName("GetPayment")
.Produces<PaymentDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status499ClientClosedRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.Run();


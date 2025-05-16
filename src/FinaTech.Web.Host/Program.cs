using FinaTech.Application;
using FinaTech.Application.Services.Payment;
using FinaTech.Application.Services.Payment.Dto;
using FinaTech.Web.Host.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<PaymentErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint for GET /payments/{id}
// Retrieves a single payment by ID
app.MapGet("/payments/{id:int}", async (int id, IPaymentService paymentService, CancellationToken cancellationToken) =>
{
    var payment = await paymentService.GetPaymentAsync(id, cancellationToken);

    if (payment == null)
    {
        return Results.NotFound($"Payment with ID {id} not found.");
    }

    return Results.Ok(payment);
})
.WithName("GetPayment")
.WithTags("Payment")
.Produces<PaymentDto>()
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status499ClientClosedRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapPost("/payments", async (IPaymentService paymentService, CreatePaymentDto payment, CancellationToken cancellationToken) =>
{
    var createdPayment = await paymentService.CreatePaymentAsync(payment, cancellationToken);
    return Results.Created($"/payments/{createdPayment.Id}", createdPayment);
})
.WithName("Create")
.WithTags("Payment")
.Produces<PaymentDto>(StatusCodes.Status201Created) // Document 201 response
.Produces(StatusCodes.Status400BadRequest) // Document validation errors
.Produces(StatusCodes.Status409Conflict) // Document resource already exists error
.Produces(StatusCodes.Status499ClientClosedRequest) // Document cancellation
.Produces(StatusCodes.Status500InternalServerError); // Document potential 500 errors

app.Run();


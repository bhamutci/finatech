using FinaTech.Application;
using FinaTech.Application.Services.Dto;
using FinaTech.Application.Services.Payment;
using FinaTech.Application.Services.Payment.Dto;
using FinaTech.Web.Host.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

string[] enabledCorsOrigins =
    builder.Configuration["Application:CorsOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries) ??
    [];

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policy =>
        policy.WithOrigins(enabledCorsOrigins)
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<PaymentErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

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
.Produces<Payment>()
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status499ClientClosedRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/payments",
        async (IPaymentService paymentService, [AsParameters] PaymentFilter filter,
            CancellationToken cancellationToken) =>
        {
            var pagedResult = await paymentService.GetPaymentsAsync(filter, cancellationToken);

            return Results.Ok(pagedResult);
        })
    .WithName("GetPayments")
    .WithTags("Payment")
    .Produces<PagedResult<ListPayment>>()
    .Produces(StatusCodes.Status499ClientClosedRequest)
    .Produces(StatusCodes.Status500InternalServerError);

app.MapPost("/payments",
        async (IPaymentService paymentService, CreatePayment payment, CancellationToken cancellationToken) =>
        {
            var createdPayment = await paymentService.CreatePaymentAsync(payment, cancellationToken);
            return Results.Created($"/payments/{createdPayment.Id}", createdPayment);
        })
.WithName("Create")
.WithTags("Payment")
.Produces<Payment>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status499ClientClosedRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.Run();



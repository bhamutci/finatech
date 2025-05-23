using FinaTech.Web.Blazor.Components;
using FinaTech.Web.Blazor.Model;
using FinaTech.Web.Blazor.Model.Validator;
using FinaTech.Web.Blazor.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor(c => c.DetailedErrors = true);

string hostAddress = builder.Configuration["Application:Host"] ?? string.Empty;
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(hostAddress);
});

builder.Services.AddSingleton<IValidator<PaymentModel>, PaymentValidator>();
builder.Services.AddSingleton<IValidator<AccountModel>, AccountValidator>();
builder.Services.AddSingleton<IValidator<AddressModel>, AddressValidator>();
builder.Services.AddSingleton<IValidator<MoneyModel>, MoneyValidator>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

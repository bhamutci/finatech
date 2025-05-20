using FinaTech.Web.Blazor.Components;
using FinaTech.Web.Blazor.Model;
using FinaTech.Web.Blazor.Model.Validator;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddScoped<IValidator<PaymentModel>, PaymentValidator>();
builder.Services.AddScoped<IValidator<AccountModel>, AccountValidator>();
builder.Services.AddScoped<IValidator<AddressModel>, AddressValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

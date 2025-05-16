using FinaTech.Web.Blazor.Components;
using FinaTech.Web.Blazor.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

string hostAddress = builder.Configuration["Application:Host"] ?? string.Empty;
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(hostAddress);
});

// Configure the HTTP request pipeline.
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

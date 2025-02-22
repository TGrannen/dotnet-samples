using BlazorApp1.Components;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var config = builder.Configuration.GetSection("AIChat");
builder.Services.AddChatClient(new OllamaChatClient(config.GetValue<string>("Endpoint")!, config.GetValue<string>("ModelId")!))
    .UseLogging();
    // .UseFunctionInvocation()
    // .UseDistributedCache()
    // .UseOpenTelemetry();

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
using MudBlazor.Services;
using Outbox.SampleBlazor;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

await builder.BuildLocalStackContainer();

builder.Services.AddTransient<IMessagePublisher, FakeMessagePublisher>();
builder.Services.AddDynamoDb(builder.Configuration);
builder.Services.AddDynamoDbOutbox();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var seeder = app.Services.GetService<ISeeder>();
if (seeder != null)
    await seeder.Initialize();

app.Run();
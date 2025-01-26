using Messaging.RabbitMQ.AdminBlazor.Services;
using MudBlazor.Services;
using Serilog;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddSingleton<PublisherBackgroundServiceConfig>();
builder.Services.AddHostedService<PublisherBackgroundService>();

var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:Host");
bool useWolverine = builder.Configuration.GetValue("UseWolverine", false);
if (useWolverine)
{
    builder.Services.AddTransient<IPublisher, WolverinePublisher>();
    builder.Host.UseWolverine(opts =>
    {
        opts.PublishMessage<TestMessage>().ToRabbitExchange("test-message");
        
        // Configure Rabbit MQ connection properties programmatically
        // against a ConnectionFactory
        opts.UseRabbitMq(rabbit =>
            {
                // Using a local installation of Rabbit MQ
                // via a running Docker image
                rabbit.HostName = rabbitHost;
            })
            // Directs Wolverine to build any declared queues, exchanges, or
            // bindings with the Rabbit MQ broker as part of bootstrapping time
            .AutoProvision()
            .UseConventionalRouting(x =>
            {
                // Customize the naming convention for the outgoing exchanges
                x.ExchangeNameForSending(type => type.Name + "Exchange");

                // Customize the naming convention for incoming queues
                x.QueueNameForListener(type => type.FullName.Replace('.', '-'));
            });
    });
}
else
{
    SetupMassTransit(builder, rabbitHost);
}

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

app.Run();

await Log.CloseAndFlushAsync();

void SetupMassTransit(WebApplicationBuilder webApplicationBuilder, string? rabbitHost)
{
    webApplicationBuilder.Services.AddTransient<IPublisher, MassTransitPublisher>();

    webApplicationBuilder.Services.AddMassTransit(x =>
    {
        // x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("dev", false));

        x.AddInMemoryInboxOutbox();
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitHost, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    });
}
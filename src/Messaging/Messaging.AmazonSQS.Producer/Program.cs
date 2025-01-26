using MassTransit;
using Messaging.Configuration;
using Messaging.Configuration.Models;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messaging.AmazonSQS.Producer", Version = "v1" }); });

builder.Services.AddMassTransit(x =>
{
    x.UsingAmazonSqs((context, cfg) =>
    {
        var config = context.GetRequiredService<IAwsSqsConfig>();
        cfg.Host(config.HostName, h =>
        {
            h.AccessKey(config.AccessKey);
            h.SecretKey(config.SecretKey);

            //// following are OPTIONAL

            //// specify a scope for all queues
            //h.Scope("dev");

            //// scope topics as well
            //h.EnableScopedTopics();
        });
    });
});

builder.Services.AddAwsSqsConfiguration(builder.Configuration);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging.AmazonSQS.Producer v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
await app.RunAsync();
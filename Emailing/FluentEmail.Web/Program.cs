using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "FluentEmail.Web", Version = "v1" }); });

/*
 * **********************************************************
 * Fluent Email Config
 */
var configuration = builder.Configuration;
var fromAddress = configuration.GetValue("EmailConfig:FromAddress", "default-from@test.com");
//var apiKey = configuration.GetValue("EmailConfig:SendGrid:APIKey", "dummy-api-key");
//var sandBoxMode = configuration.GetValue("EmailConfig:SendGrid:UseSandbox", true);

var server = configuration.GetValue<string>("EmailConfig:SMTPConfig:Server");
var port = configuration.GetValue<int>("EmailConfig:SMTPConfig:Port");

builder.Services
    .AddFluentEmail(fromAddress)
    .AddRazorRenderer(Path.Combine(builder.Environment.ContentRootPath, "Templates/"))
    //.AddSendGridSender(apiKey, sandBoxMode);
    .AddSmtpSender(server, port);

/*************************************************************/

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FluentEmail.Web v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();
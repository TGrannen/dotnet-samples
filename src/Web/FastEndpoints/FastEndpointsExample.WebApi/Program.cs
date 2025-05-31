using FastEndpoints.Swagger;
using FastEndpointsExample.WebApi.Endpoints;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddFastEndpoints(o => { o.Assemblies = new[] { typeof(CreateUserEndpoint).Assembly }; })
    .SwaggerDocument();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseFastEndpoints()
    .UseSwaggerGen();

if (app.Environment.IsDevelopment())
{
    //scalar by default looks for the swagger json file here: 
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.Run();
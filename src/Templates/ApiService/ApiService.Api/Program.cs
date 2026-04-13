using ApiService.Api.Common;
using ApiService.Api.Common.Web.ErrorHandling;
using ApiService.Api.Common.Web.Versioning;
using ApiService.Api.Infrastructure.Persistence;
using ApiService.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddCaching();
builder.Services.AddOpenApiWithFullNames();
builder.Services.AddApiVersioningServices();
builder.Services.AddPersistence(builder.Configuration);
builder.AddPersistenceInstrumentation();
builder.Services.AddApplication();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidJsonRequestBodyExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.UseHttpsRedirection();

app.MapVersionedApiEndpoints(typeof(ApiService.Api.Program).Assembly);

app.Run();

namespace ApiService.Api
{
    public partial class Program;
}

using Microsoft.FeatureManagement;
using Scalar.AspNetCore;
using StranglerPattern.ApiService.FeatureFlagging;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFeatureManagement();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(opt => { opt.Servers = []; });
}

app.MapControllers();
app.MapDefaultEndpoints();
app.UseMiddleware<FeatureFlagMiddleware>();
app.MapReverseProxy();
app.Run();
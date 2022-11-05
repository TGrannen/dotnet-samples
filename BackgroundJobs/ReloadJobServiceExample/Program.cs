using Microsoft.AspNetCore.Mvc;
using ReloadJobServiceExample.Services;
using ReloadJobServiceExample.Testing;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJobs<Program>();
builder.Services.AddSingleton<ResultProvider>();

var app = builder.Build();

app.MapPost("/reload", ([FromServices] IEnumerable<IReloadJobService> jobBases) =>
{
    foreach (var jobBase in jobBases)
    {
        jobBase.SetReload();
    }

    return Task.FromResult(Results.Ok());
});

app.MapPost("/stop", ([FromServices] IEnumerable<IReloadJobService> jobBases) =>
{
    foreach (var jobBase in jobBases)
    {
        jobBase.Stop();
    }

    return Task.FromResult(Results.Ok());
});

app.MapPost("/result/{value}", (bool value, [FromServices] ResultProvider resultProvider) =>
{
    resultProvider.Result = value;

    return Task.FromResult(Results.Ok());
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
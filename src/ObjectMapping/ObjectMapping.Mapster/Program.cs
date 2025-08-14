using Bogus;
using Mapster;
using ObjectMapping.Mapster.Models;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMapster();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

var autoFaker = new Faker<User>()
    .RuleFor(x => x.Id, f => f.Random.Int(1_000_000, 999_999_999))
    .RuleFor(x => x.FirstName, f => f.Person.FirstName)
    .RuleFor(x => x.LastName, f => f.Person.LastName)
    .RuleFor(x => x.EmailAddress, f => f.Person.Email)
    .RuleFor(x => x.Age, f => f.Random.Int(15, 90));

app.MapGet("/TestMapster", () => autoFaker.Generate(1).First().Adapt<UserDto>());
app.MapGet("/TestMapster/Multiple", (int numberOfUsers = 5) => autoFaker.Generate(numberOfUsers).Select(x => x.Adapt<UserDto>()));

app.Run();
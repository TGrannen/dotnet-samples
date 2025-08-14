using AutoBogus;
using Bogus;
using Facet.Extensions;
using Microsoft.AspNetCore.Mvc;
using ObjectMapping.Facets.Models;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var autoFaker = new Faker<User>()
    .RuleFor(x => x.Id, f => f.Random.Int(1_000_000, 999_999_999))
    .RuleFor(x => x.FirstName, f => f.Person.FirstName)
    .RuleFor(x => x.LastName, f => f.Person.LastName)
    .RuleFor(x => x.EmailAddress, f => f.Person.Email)
    .RuleFor(x => x.Age, f => f.Random.Int(15, 90));

app.MapGet("/TestMapper", () => autoFaker.Generate(1).First().ToFacet<User, UserDto>());
app.MapGet("/TestMapper/Multiple", (int numberOfUsers = 5) => autoFaker.Generate(numberOfUsers).SelectFacets<User, UserDto>());

app.MapGet("Dto2/TestMapper", () => UserDto2.Map(autoFaker.Generate(1).First()));
app.MapGet("Dto2/TestMapper/Multiple", (int numberOfUsers = 5) => autoFaker.Generate(numberOfUsers).Select(UserDto2.Map));

app.Run();
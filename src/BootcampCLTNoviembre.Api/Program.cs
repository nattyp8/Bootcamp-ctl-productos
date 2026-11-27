using System.Text.Json.Serialization;
using BootcampCLTNoviembre.Api.Endpoints;
using BootcampCLTNoviembre.Api.ExceptionHandling;
using BootcampCLTNoviembre.Application;
using BootcampCLTNoviembre.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Registra los casos de uso y sus implementaciones de almacenamiento.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();


var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapProductsEndpoints();

app.Run();

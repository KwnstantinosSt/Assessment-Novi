using Currency.Exchange;
using Currency.Exchange.Common.Endpoints;
using Currency.Exchange.Common.Microservices;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.SetupMicroservices();
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapHealthChecks(pattern: "/health");
app.MapMinimalEndpoints();
app.Run();

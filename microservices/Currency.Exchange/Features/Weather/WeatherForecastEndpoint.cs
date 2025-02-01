// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;

namespace Currency.Exchange.Features.Weather;

public class WeatherForecastEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet(pattern: "/weatherforecast",
                () =>
                {
                    var forecast = Enumerable.Range(start: 1, count: 5)
                        .Select(index =>
                            new WeatherForecast(
                                Date: DateOnly.FromDateTime(dateTime: DateTime.Now.AddDays(index)),
                                TemperatureC: Random.Shared.Next(minValue: -20, maxValue: 55),
                                Summary: summaries[Random.Shared.Next(summaries.Length)]))
                        .ToArray();
                    return forecast;
                })
            .WithName(endpointName: "GetWeatherForecast");
    }
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

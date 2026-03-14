using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Weather;
using Pew.Dashboard.Application.Features.Weather.GetWeather;

namespace Pew.Dashboard.Api.Endpoints.Weather;

public sealed class GetWeatherEndpoint(ISender sender) : Endpoint<EmptyRequest, ApiResponse<WeatherResponse>>
{
    public override void Configure()
    {
        Get("/api/weather");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get current weather and forecast";
            s.Description = "Returns current weather conditions and a 5-day forecast from Open-Meteo";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetWeatherQuery(), ct);
        await Send.ResponseAsync(result.ToApiResponse(), result.ToHttpStatusCode(), ct);
    }
}

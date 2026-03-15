using Ardalis.Result;
using MediatR;
using Pew.Dashboard.Application.Common.Interfaces;

namespace Pew.Dashboard.Application.Features.Weather.GetWeather;

public sealed record GetWeatherQuery : IRequest<Result<WeatherResponse>>
{
    public sealed class Handler(IWeatherService weatherService) : IRequestHandler<GetWeatherQuery, Result<WeatherResponse>>
    {
        public async Task<Result<WeatherResponse>> Handle(GetWeatherQuery request, CancellationToken ct)
        {
            var response = await weatherService.GetWeatherAsync(ct);
            return Result.Success(response);
        }
    }
}

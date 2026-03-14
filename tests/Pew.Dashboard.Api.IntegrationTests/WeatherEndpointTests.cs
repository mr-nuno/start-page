using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Weather;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class WeatherEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetWeather_Should_Return200WithWeatherData_When_Called()
    {
        var response = await Client.GetAsync("/api/weather");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<WeatherResponse>("/api/weather");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Description.ShouldNotBeNullOrEmpty();
        result.Data.Temperature.ShouldBe(22.5);
    }
}

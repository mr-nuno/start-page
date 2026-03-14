using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Guitar;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class GuitarEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetGuitar_Should_Return200WithChordDataIncludingSvg_When_Called()
    {
        var response = await Client.GetAsync("/api/guitar");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<GuitarResponse>("/api/guitar");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldNotBeNullOrEmpty();
        result.Data.Svg.ShouldNotBeNullOrEmpty();
        result.Data.Svg.ShouldContain("<svg");
    }
}

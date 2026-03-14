using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Sports;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class SportsEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetSports_Should_Return200WithTeamInfo_When_Called()
    {
        var response = await Client.GetAsync("/api/sports");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<SportsResponse>("/api/sports");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Team.ShouldNotBeNullOrEmpty();
        result.Data.League.ShouldNotBeNullOrEmpty();
    }
}

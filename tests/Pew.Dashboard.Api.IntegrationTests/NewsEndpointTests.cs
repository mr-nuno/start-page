using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.News;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class NewsEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetNews_Should_Return200WithLocalAndGlobalArrays_When_Called()
    {
        var response = await Client.GetAsync("/api/news");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<NewsResponse>("/api/news");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Local.ShouldNotBeNull();
        result.Data.Local.Count.ShouldBeGreaterThan(0);
        result.Data.Global.ShouldNotBeNull();
        result.Data.Global.Count.ShouldBeGreaterThan(0);
    }
}

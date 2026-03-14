using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Comic;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class ComicEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetComic_Should_Return200WithComicData_When_Called()
    {
        var response = await Client.GetAsync("/api/comic");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<ComicResponse>("/api/comic");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldNotBeNullOrEmpty();
        result.Data.PageUrl.ShouldNotBeNullOrEmpty();
    }
}

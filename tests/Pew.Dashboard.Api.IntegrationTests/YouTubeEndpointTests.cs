using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.YouTube;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class YouTubeEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetYouTube_Should_Return200WithVideoData_When_Called()
    {
        var response = await Client.GetAsync("/api/youtube");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<YouTubeResponse>("/api/youtube");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldNotBeNullOrEmpty();
        result.Data.VideoId.ShouldNotBeNullOrEmpty();
        result.Data.Link.ShouldNotBeNullOrEmpty();
    }
}

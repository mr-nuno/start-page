using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Song;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class SongEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetSong_Should_Return200WithSongData_When_Called()
    {
        var response = await Client.GetAsync("/api/song");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<SongResponse>("/api/song");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldNotBeNullOrEmpty();
        result.Data.Artist.ShouldNotBeNullOrEmpty();
    }
}

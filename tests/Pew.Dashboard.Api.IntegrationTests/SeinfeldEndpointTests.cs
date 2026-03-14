using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Seinfeld;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class SeinfeldEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetSeinfeld_Should_Return200WithQuote_When_Called()
    {
        var response = await Client.GetAsync("/api/seinfeld");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<SeinfeldResponse>("/api/seinfeld");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Quote.ShouldNotBeNullOrEmpty();
        result.Data.Character.ShouldNotBeNullOrEmpty();
    }
}

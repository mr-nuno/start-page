using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.Quote;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class QuoteEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetQuote_Should_Return200WithQuote_When_Called()
    {
        var response = await Client.GetAsync("/api/quote");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<QuoteResponse>("/api/quote");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Quote.ShouldNotBeNullOrEmpty();
        result.Data.Source.ShouldNotBeNullOrEmpty();
    }
}

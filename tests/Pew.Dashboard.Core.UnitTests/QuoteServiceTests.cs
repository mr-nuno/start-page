using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Quote;
using Pew.Dashboard.Infrastructure.Services.Quote;
using Shouldly;

namespace Pew.Dashboard.Core.UnitTests;

public sealed class QuoteServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly HttpClient _httpClient;
    private readonly QuoteService _sut;

    public QuoteServiceTests()
    {
        _httpClient = new HttpClient(new FakeQuoteHandler());
        _httpClient.BaseAddress = new Uri("https://zenquotes.io");
        _sut = new QuoteService(_httpClient, _cache, _dateTimeProvider);
    }

    [Fact]
    public async Task GetQuoteOfTheDayAsync_Should_ReturnQuoteFromApi_When_ApiResponds()
    {
        var result = await _sut.GetQuoteOfTheDayAsync(CancellationToken.None);

        result.ShouldNotBeNull();
        result.Quote.ShouldBe("Test quote from API");
        result.Source.ShouldBe("Test Author");
    }

    [Fact]
    public async Task GetQuoteOfTheDayAsync_Should_ReturnCachedQuote_When_CalledTwice()
    {
        var first = await _sut.GetQuoteOfTheDayAsync(CancellationToken.None);
        var second = await _sut.GetQuoteOfTheDayAsync(CancellationToken.None);

        second.ShouldBe(first);
    }

    [Fact]
    public async Task GetQuoteOfTheDayAsync_Should_ReturnFallbackQuote_When_ApiFails()
    {
        var failingClient = new HttpClient(new FailingHandler());
        failingClient.BaseAddress = new Uri("https://zenquotes.io");
        var fixedDate = new DateTimeOffset(2026, 3, 14, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProvider.UtcNow.Returns(fixedDate);

        var sut = new QuoteService(failingClient, _cache, _dateTimeProvider);
        var result = await sut.GetQuoteOfTheDayAsync(CancellationToken.None);

        result.ShouldNotBeNull();
        result.Quote.ShouldNotBeNullOrEmpty();
        result.Source.ShouldNotBeNullOrEmpty();
    }

    private sealed class FakeQuoteHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var json = """[{"q":"Test quote from API","a":"Test Author","h":"<blockquote>Test</blockquote>"}]""";
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class FailingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
    }
}

using NSubstitute;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Quote;
using Pew.Dashboard.Infrastructure.Services.Quote;
using Shouldly;

namespace Pew.Dashboard.Core.UnitTests;

public sealed class QuoteServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly QuoteService _sut;

    public QuoteServiceTests()
    {
        _sut = new QuoteService(_dateTimeProvider);
    }

    [Fact]
    public void GetQuoteOfTheDay_Should_ReturnQuote_When_Called()
    {
        var fixedDate = new DateTimeOffset(2026, 3, 14, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProvider.UtcNow.Returns(fixedDate);

        var result = _sut.GetQuoteOfTheDay();

        result.ShouldNotBeNull();
        result.Quote.ShouldNotBeNullOrEmpty();
        result.Source.ShouldNotBeNullOrEmpty();
    }
}

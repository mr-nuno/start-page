using NSubstitute;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Seinfeld;
using Pew.Dashboard.Infrastructure.Services.Seinfeld;
using Shouldly;

namespace Pew.Dashboard.Core.UnitTests;

public sealed class SeinfeldServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly SeinfeldService _sut;

    public SeinfeldServiceTests()
    {
        _sut = new SeinfeldService(_dateTimeProvider);
    }

    [Fact]
    public void GetQuoteOfTheDay_Should_ReturnQuote_When_Called()
    {
        var fixedDate = new DateTimeOffset(2026, 3, 14, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProvider.UtcNow.Returns(fixedDate);

        var result = _sut.GetQuoteOfTheDay();

        result.ShouldNotBeNull();
        result.Quote.ShouldNotBeNullOrEmpty();
        result.Character.ShouldNotBeNullOrEmpty();
        result.Episode.ShouldNotBeNullOrEmpty();
    }
}

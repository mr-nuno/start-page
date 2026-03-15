using NSubstitute;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Guitar;
using Pew.Dashboard.Infrastructure.Services.Guitar;
using Shouldly;

namespace Pew.Dashboard.Core.UnitTests;

public sealed class GuitarServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly GuitarService _sut;

    public GuitarServiceTests()
    {
        _sut = new GuitarService(_dateTimeProvider);
    }

    [Fact]
    public void GetChordOfTheDay_Should_ReturnConsistentChord_When_SameDateUsed()
    {
        var fixedDate = new DateTimeOffset(2026, 3, 14, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProvider.UtcNow.Returns(fixedDate);

        var first = _sut.GetChordOfTheDay();
        var second = _sut.GetChordOfTheDay();

        first.ShouldNotBeNull();
        second.ShouldNotBeNull();
        first.Name.ShouldBe(second.Name);
        first.Short.ShouldBe(second.Short);
        first.Svg.ShouldBe(second.Svg);
    }

    [Fact]
    public void GetChordOfTheDay_Should_ReturnValidSvg_When_Called()
    {
        var fixedDate = new DateTimeOffset(2026, 3, 14, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProvider.UtcNow.Returns(fixedDate);

        var result = _sut.GetChordOfTheDay();

        result.ShouldNotBeNull();
        result.Svg.ShouldNotBeNullOrEmpty();
        result.Svg.ShouldContain("<svg");
    }
}

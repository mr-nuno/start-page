using Pew.Dashboard.Application.Common.Interfaces;

namespace Pew.Dashboard.Application.Features.Seinfeld;

public sealed class SeinfeldService(IDateTimeProvider dateTimeProvider) : ISeinfeldService
{
    public SeinfeldResponse GetQuoteOfTheDay()
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var index = today.DayNumber % SeinfeldQuotes.Quotes.Length;
        var quote = SeinfeldQuotes.Quotes[index];

        return new SeinfeldResponse(
            Quote: quote.Quote,
            Character: quote.Character,
            Episode: quote.Episode);
    }
}

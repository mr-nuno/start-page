using Pew.Dashboard.Application.Common.Interfaces;

namespace Pew.Dashboard.Application.Features.Quote;

public sealed class QuoteService(IDateTimeProvider dateTimeProvider) : IQuoteService
{
    public QuoteResponse GetQuoteOfTheDay()
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var index = today.DayNumber % QuoteLibrary.Quotes.Length;
        var quote = QuoteLibrary.Quotes[index];

        return new QuoteResponse(
            Quote: quote.Quote,
            Source: quote.Source);
    }
}

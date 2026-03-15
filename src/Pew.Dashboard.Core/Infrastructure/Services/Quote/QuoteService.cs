using Pew.Dashboard.Application.Common.Interfaces;

using Pew.Dashboard.Application.Features.Quote;

namespace Pew.Dashboard.Infrastructure.Services.Quote;

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

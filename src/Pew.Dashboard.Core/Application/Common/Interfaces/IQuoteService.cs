using Pew.Dashboard.Application.Features.Quote;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponse> GetQuoteOfTheDayAsync(CancellationToken ct);
}

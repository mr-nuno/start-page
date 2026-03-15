using Pew.Dashboard.Application.Features.Quote;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IQuoteService
{
    QuoteResponse GetQuoteOfTheDay();
}

using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Quote.GetQuote;

public sealed record GetQuoteQuery : IRequest<Result<QuoteResponse>>
{
    public sealed class Handler(IQuoteService quoteService) : IRequestHandler<GetQuoteQuery, Result<QuoteResponse>>
    {
        public Task<Result<QuoteResponse>> Handle(GetQuoteQuery request, CancellationToken ct)
        {
            var response = quoteService.GetQuoteOfTheDay();
            return Task.FromResult(Result<QuoteResponse>.Success(response));
        }
    }
}

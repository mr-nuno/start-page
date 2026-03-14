using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Seinfeld.GetSeinfeld;

public sealed record GetSeinfeldQuery : IRequest<Result<SeinfeldResponse>>
{
    public sealed class Handler(ISeinfeldService seinfeldService) : IRequestHandler<GetSeinfeldQuery, Result<SeinfeldResponse>>
    {
        public Task<Result<SeinfeldResponse>> Handle(GetSeinfeldQuery request, CancellationToken ct)
        {
            var response = seinfeldService.GetQuoteOfTheDay();
            return Task.FromResult(Result<SeinfeldResponse>.Success(response));
        }
    }
}

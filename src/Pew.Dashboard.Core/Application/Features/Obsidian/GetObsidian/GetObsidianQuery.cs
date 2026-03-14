using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Obsidian.GetObsidian;

public sealed record GetObsidianQuery : IRequest<Result<ObsidianResponse>>
{
    public sealed class Handler(IVaultService vaultService) : IRequestHandler<GetObsidianQuery, Result<ObsidianResponse>>
    {
        public async Task<Result<ObsidianResponse>> Handle(GetObsidianQuery request, CancellationToken ct)
        {
            var response = await vaultService.GetDashboardAsync(ct);
            return Result<ObsidianResponse>.Success(response);
        }
    }
}

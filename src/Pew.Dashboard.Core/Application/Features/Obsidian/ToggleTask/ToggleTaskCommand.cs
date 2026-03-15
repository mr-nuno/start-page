using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using MediatR;

namespace Pew.Dashboard.Application.Features.Obsidian.ToggleTask;

public sealed record ToggleTaskCommand(string FilePath, int LineNumber) : IRequest<Result>
{
    public sealed class Handler(IVaultService vaultService) : IRequestHandler<ToggleTaskCommand, Result>
    {
        public async Task<Result> Handle(ToggleTaskCommand request, CancellationToken ct)
        {
            return await vaultService.ToggleTaskAsync(request.FilePath, request.LineNumber, ct);
        }
    }

    public sealed class Validator : Validator<ToggleTaskCommand>
    {
        public Validator()
        {
            RuleFor(x => x.FilePath).NotEmpty().WithMessage("File path is required");
            RuleFor(x => x.LineNumber).GreaterThan(0).WithMessage("Line number must be positive");
        }
    }
}

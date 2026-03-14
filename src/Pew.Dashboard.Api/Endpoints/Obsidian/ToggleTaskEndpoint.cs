using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Obsidian.ToggleTask;

namespace Pew.Dashboard.Api.Endpoints.Obsidian;

public sealed class ToggleTaskEndpoint(ISender sender) : Endpoint<ToggleTaskCommand, ApiResponse<object>>
{
    public override void Configure()
    {
        Post("/api/obsidian/toggle");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Toggle task checkbox";
            s.Description = "Toggles a task checkbox between checked and unchecked in the specified vault file.";
            s.ExampleRequest = new ToggleTaskCommand("Daily Notes/2026-03-14.md", 5);
        });
        Options(x => x.WithTags("Obsidian"));
    }

    public override async Task HandleAsync(ToggleTaskCommand req, CancellationToken ct)
    {
        var result = await sender.Send(req, ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}

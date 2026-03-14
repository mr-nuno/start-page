using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Obsidian.AppendNote;

namespace Pew.Dashboard.Api.Endpoints.Obsidian;

public sealed class AppendNoteEndpoint(ISender sender) : Endpoint<AppendNoteCommand, ApiResponse<object>>
{
    public override void Configure()
    {
        Post("/api/obsidian/note");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Append note to daily note";
            s.Description = "Appends text to today's daily note, creating the file if it doesn't exist.";
            s.ExampleRequest = new AppendNoteCommand("Remember to check the logs");
        });
        Options(x => x.WithTags("Obsidian"));
    }

    public override async Task HandleAsync(AppendNoteCommand req, CancellationToken ct)
    {
        var result = await sender.Send(req, ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}

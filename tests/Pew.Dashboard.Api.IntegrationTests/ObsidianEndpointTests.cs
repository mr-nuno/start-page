using System.Net;
using System.Net.Http.Json;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Obsidian;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class ObsidianEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetObsidian_Should_Return200WithDashboardData_When_Called()
    {
        var response = await Client.GetAsync("/api/obsidian");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<ObsidianResponse>("/api/obsidian");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.DailyNote.ShouldNotBeNullOrEmpty();
        result.Data.Date.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task AppendNote_Should_Return400_When_TextIsEmpty()
    {
        var response = await Client.PostAsJsonAsync("/api/obsidian/note", new { Text = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        result.ShouldNotBeNull();
        result.Success.ShouldBe(false);
        result.ValidationErrors.ShouldNotBeNull();
        result.ValidationErrors.Count.ShouldBeGreaterThan(0);
    }
}

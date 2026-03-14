using System.Net;
using Pew.Dashboard.Api.IntegrationTests.Common;
using Pew.Dashboard.Application.Features.SystemInfo;
using Shouldly;

namespace Pew.Dashboard.Api.IntegrationTests;

public sealed class SystemEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetSystem_Should_Return200WithSystemInfo_When_Called()
    {
        var response = await Client.GetAsync("/api/system");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await Client.GetApiAsync<SystemResponse>("/api/system");

        result.ShouldNotBeNull();
        result.Success.ShouldBe(true);
        result.Data.ShouldNotBeNull();
        result.Data.Memory.ShouldNotBeNull();
        result.Data.Disk.ShouldNotBeNull();
        result.Data.Cpu.ShouldNotBeNull();
    }
}

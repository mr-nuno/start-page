using System.Net.Http.Json;
using Pew.Dashboard.Application.Common.Models;

namespace Pew.Dashboard.Api.IntegrationTests.Common;

public static class HttpClientExtensions
{
    public static async Task<ApiResponse<T>?> GetApiAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
    }
}

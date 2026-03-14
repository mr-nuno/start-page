namespace Pew.Dashboard.Api.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected HttpClient Client { get; }
    protected IntegrationTestWebAppFactory Factory { get; }

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}

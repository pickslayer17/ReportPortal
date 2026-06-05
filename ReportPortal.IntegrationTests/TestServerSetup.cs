using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Assembly-level setup: spin up ONE API host + test database for the whole run (expensive to
/// create per fixture), then create the schema once. Individual tests wipe rows between
/// themselves via <see cref="ApiFactory.ResetAsync"/>.
/// </summary>
[SetUpFixture]
public class TestServerSetup
{
    public static ApiFactory Factory { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        Factory = new ApiFactory();
        await Factory.CreateSchemaAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await Factory.DropAsync();
        Factory.Dispose();
    }
}

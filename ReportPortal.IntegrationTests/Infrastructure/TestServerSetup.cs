using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Assembly-level setup: spin up ONE API host for the whole run. Unlike a classic isolated suite,
/// the test database is NOT created or dropped here and is NOT wiped between tests — it persists
/// across runs. The workflow is manual and two-step:
///   1) run the Initialize category once   (dotnet test --filter "Category=Initialize")
///      -> builds the schema from scratch and seeds the shared baseline (admin/project/user/subproject);
///   2) run Smoke / SmokeE2E as often as you like against that baseline.
/// Booting the host still ensures the schema exists (the app calls EnsureCreated on startup in
/// Development), so a Smoke run never crashes on a missing table — it just needs Initialize to
/// have seeded the baseline first.
/// </summary>
[SetUpFixture]
public class TestServerSetup
{
    public static ApiFactory Factory { get; private set; } = null!;

    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        Factory = new ApiFactory();
        // Force host startup so the app materialises the schema (EnsureCreated) if it is missing.
        Factory.CreateClient().Dispose();
    }

    [OneTimeTearDown]
    public void GlobalTearDown() => Factory.Dispose();
}

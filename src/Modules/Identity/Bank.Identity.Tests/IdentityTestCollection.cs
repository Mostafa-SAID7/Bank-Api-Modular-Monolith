namespace Bank.Identity.Tests;

/// <summary>
/// XUnit collection for Identity integration tests
/// Ensures tests are run sequentially to avoid database conflicts
/// </summary>
[CollectionDefinition("Identity Integration")]
public class IdentityTestCollection : ICollectionFixture<IdentityTestFixture>
{
}

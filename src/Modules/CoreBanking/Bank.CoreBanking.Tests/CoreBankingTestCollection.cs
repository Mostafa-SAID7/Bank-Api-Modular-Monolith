namespace Bank.CoreBanking.Tests;

/// <summary>
/// XUnit collection for CoreBanking integration tests
/// Ensures tests are run sequentially to avoid DB conflicts
/// </summary>
[CollectionDefinition("CoreBanking Integration")]
public class CoreBankingTestCollection : ICollectionFixture<CoreBankingTestFixture>
{
}

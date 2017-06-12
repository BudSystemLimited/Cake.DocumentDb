using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Seed
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbseedtest";
    }
}

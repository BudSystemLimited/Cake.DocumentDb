using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Database
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbtest";
    }
}

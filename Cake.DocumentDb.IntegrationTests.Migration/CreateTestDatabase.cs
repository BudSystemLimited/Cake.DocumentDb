using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbmigrationtest";
    }
}

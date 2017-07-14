using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Deletions
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbdeletiontest";
    }
}

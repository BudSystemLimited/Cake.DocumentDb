using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Collection
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbcoltest";
    }
}

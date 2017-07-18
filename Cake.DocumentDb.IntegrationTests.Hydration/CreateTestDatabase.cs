using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Hydration
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbhydrationtest";
    }
}

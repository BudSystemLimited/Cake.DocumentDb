using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Seed
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbseedtest";
    }
}

using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Database
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbtest";
    }
}

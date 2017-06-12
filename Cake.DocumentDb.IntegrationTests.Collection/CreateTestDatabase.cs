using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Collection
{
    public class CreateTestDatabase : ICreateDocumentDatabase
    {
        public string Name => "cakeddbcoltest";
    }
}

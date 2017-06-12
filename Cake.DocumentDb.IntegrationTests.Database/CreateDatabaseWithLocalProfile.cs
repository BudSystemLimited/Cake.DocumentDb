using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Database
{
    [Profile("local")]
    public class CreateDatabaseWithLocalProfile : ICreateDocumentDatabase
    {
        public string Name => "cakeddbtestlocal";
    }
}

using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Database;

namespace Cake.DocumentDb.IntegrationTests.Database
{
    [Profile("random")]
    public class CreateDatabaseWithRandomProfileWillNotCreate : ICreateDocumentDatabase
    {
        public string Name => "cakeddbtestrandom";
    }
}
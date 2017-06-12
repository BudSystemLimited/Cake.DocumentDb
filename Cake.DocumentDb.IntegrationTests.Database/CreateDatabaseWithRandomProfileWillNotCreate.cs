using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Database
{
    [Profile("random")]
    public class CreateDatabaseWithRandomProfileWillNotCreate : ICreateDocumentDatabase
    {
        public string Name => "cakeddbtestrandom";
    }
}
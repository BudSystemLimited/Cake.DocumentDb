using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Seed
{
    public class CreateTestCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbseedtest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}

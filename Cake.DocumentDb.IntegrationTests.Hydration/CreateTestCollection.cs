using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Hydration
{
    public class CreateTestCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbhydrationtest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}

using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Hydration
{
    public class CreateTestArbitraryDataCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbhydrationtest";
        public string CollectionName => "MyDataCollection";
        public string PartitionKey => "/mypartitionkey";
        public int? Throughput => 400;
    }
}

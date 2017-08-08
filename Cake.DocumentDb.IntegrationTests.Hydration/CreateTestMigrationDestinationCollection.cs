using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Hydration
{
    public class CreateTestMigrationDestinationCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbhydrationtest";
        public string CollectionName => "MyDocumentCollection";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 400;
    }
}
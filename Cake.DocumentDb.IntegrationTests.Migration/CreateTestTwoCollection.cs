using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateTestTwoCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "MyCollectionTwo";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
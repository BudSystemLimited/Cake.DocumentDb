using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateTestCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}

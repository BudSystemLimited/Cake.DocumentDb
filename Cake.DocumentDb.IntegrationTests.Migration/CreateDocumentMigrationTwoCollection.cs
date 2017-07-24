using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateDocumentMigrationTwoCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "DocumentMigrationTwo";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
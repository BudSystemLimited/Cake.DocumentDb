using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateDocumentMigrationOneCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "DocumentMigrationOne";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
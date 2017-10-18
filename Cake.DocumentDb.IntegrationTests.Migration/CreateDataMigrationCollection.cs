using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateDataMigrationCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "DataMigration";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
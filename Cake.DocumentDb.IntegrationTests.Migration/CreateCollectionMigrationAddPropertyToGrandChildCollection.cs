using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateCollectionMigrationAddPropertyToGrandChildCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "CollectionMigrationAddPropertyToGrandChild";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
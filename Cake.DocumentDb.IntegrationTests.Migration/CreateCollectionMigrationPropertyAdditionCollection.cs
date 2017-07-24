using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Migration
{
    public class CreateCollectionMigrationPropertyAdditionCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "CollectionMigrationPropertyAddition";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
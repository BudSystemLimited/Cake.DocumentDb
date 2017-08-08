using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Hydration
{
    public class CreateTestMigrationSourceCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbhydrationtest";
        public string CollectionName => "MigrationSource";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 400;
    }
}
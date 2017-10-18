using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class DataMigrationSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Document Migration One Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "DataMigration";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "datamigration.json";
    }
}
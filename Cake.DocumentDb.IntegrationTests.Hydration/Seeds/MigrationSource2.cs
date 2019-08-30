using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Seeds
{
    public class MigrationSource2 : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Migration Source 2";
        public override string Database => "cakeddbhydrationtest";
        public override string Collection => "MigrationSource";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "MigrationSource2.json";
    }
}

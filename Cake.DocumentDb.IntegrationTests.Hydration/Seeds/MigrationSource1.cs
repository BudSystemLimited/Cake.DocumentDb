using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Seeds
{
    public class MigrationSource1 : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Migration Source 1";
        public override string Database => "cakeddbhydrationtest";
        public override string Collection => "MigrationSource";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "MigrationSource1.json";
    }
}

using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Seeds
{
    public class EmbeddedSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Test";
        public override string Database => "cakeddbhydrationtest";
        public override string Collection => "MigrationSource";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseed.json";
    }
}

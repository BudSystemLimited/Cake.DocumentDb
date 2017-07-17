using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class EmbeddedSeedTwo : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Two Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseedtwo.json";
    }
}

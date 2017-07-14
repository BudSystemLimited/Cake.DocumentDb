using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Deletions.Seeds
{
    public class EmbeddedSeedTwo : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Two Test";
        public override string Database => "cakeddbdeletiontest";
        public override string Collection => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseedtwo.json";
    }
}

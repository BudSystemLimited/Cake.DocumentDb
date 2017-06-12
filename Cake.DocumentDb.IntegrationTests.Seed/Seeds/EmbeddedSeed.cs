using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Seed.Seeds
{
    public class EmbeddedSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Test";
        public override string Database => "cakeddbseedtest";
        public override string Collection => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseed.json";
    }
}

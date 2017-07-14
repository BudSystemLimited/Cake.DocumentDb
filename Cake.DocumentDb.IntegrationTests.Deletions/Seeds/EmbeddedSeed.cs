using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Deletions.Seeds
{
    public class EmbeddedSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Test";
        public override string Database => "cakeddbdeletiontest";
        public override string Collection => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseed.json";
    }
}

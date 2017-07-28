using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class MyCollectionTwoEmbeddedSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "MyCollectionTwo";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseed.json";
    }
}

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class EmbeddedSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Embedded Seed Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "embeddedseed.json";
    }
}

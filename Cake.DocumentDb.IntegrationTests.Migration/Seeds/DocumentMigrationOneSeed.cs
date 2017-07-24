using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class DocumentMigrationOneSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Document Migration One Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "DocumentMigrationOne";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "documentmigrationone.json";
    }
}

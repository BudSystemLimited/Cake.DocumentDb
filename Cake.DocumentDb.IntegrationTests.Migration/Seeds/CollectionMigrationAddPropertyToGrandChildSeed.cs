using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class CollectionMigrationAddPropertyToGrandChildSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Collection Migration Add Property To GrandChild Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "CollectionMigrationAddPropertyToGrandChild";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "collectionmigrationaddpropertytograndchild.json";
    }
}

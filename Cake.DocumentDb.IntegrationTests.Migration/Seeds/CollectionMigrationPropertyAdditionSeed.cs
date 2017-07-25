using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class CollectionMigrationPropertyAdditionSeed : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Collection Migration Property Addition Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "CollectionMigrationPropertyAddition";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "collectionmigrationpropertyaddition.json";
    }
}

using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Profile("random")]
    [Migration(201706121107)]
    public class AddNewPropertyToDocumentMigrationWithRandomProfile : IDocumentMigration
    {
        public ICakeLog Log { get; set; }
        public string Description => "Add new property to document random";
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public void Transform(dynamic item)
        {
            item.myNewRandomStringProperty = "my new random value";
        }
    }
}
using Cake.DocumentDb.Attributes;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Profile("random")]
    [Migration(201706121107)]
    public class AddNewPropertyToDocumentMigrationWithRandomProfile : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToDocumentMigrationWithRandomProfile()
        {
            Migrate(m =>
            {
                m.Description("Add new property to document local");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    item.myNewRandomStringProperty = "my new random value";
                });
            });
        }
    }
}
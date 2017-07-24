using Cake.DocumentDb.Attributes;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Profile("local")]
    [Migration(201706121107)]
    public class AddNewPropertyToDocumentMigrationWithLocalProfile : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToDocumentMigrationWithLocalProfile()
        {
            Migrate(m =>
            {
                m.Description("Add new property to document local");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    item["myNewLocalStringProperty"] = "my new local value";
                });
            });
        }
    }
}

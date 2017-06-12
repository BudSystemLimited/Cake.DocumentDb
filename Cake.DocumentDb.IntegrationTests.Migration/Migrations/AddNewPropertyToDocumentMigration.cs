using System;
using Cake.DocumentDb.Attributes;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201706121107)]
    public class AddNewPropertyToDocumentMigration : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToDocumentMigration()
        {
            Migrate(m =>
            {
                m.Description("Add new property to document");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    item.myNewStringProperty = "my new value";
                    item.myNewIntProperty = 1;
                    item.myNewBoolProperty = true;
                    item.myNewGuidProperty = Guid.NewGuid();
                });
            });
        }
    }
}

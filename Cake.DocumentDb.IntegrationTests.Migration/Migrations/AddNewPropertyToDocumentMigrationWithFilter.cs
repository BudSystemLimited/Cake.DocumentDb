using System;
using Cake.DocumentDb.Attributes;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201707171034)]
    public class AddNewPropertyToDocumentMigrationWithFilter : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToDocumentMigrationWithFilter()
        {
            Migrate(m =>
            {
                m.Description("Add new property to document with filter");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    item["myNewStringPropertyWithFilter"] = "my new value";
                    item["myNewIntPropertyWithFilter"] = 1;
                    item["myNewBoolPropertyWithFilter"] = true;
                    item["myNewGuidPropertyWithFilter"] = Guid.NewGuid();
                });
                m.Filter(doc => doc["mypartitionKey"].ToString() == "1" );
            });
        }
    }
}

using System;
using Cake.DocumentDb.Attributes;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201707241633)]
    public class AddNewPropertyToCollectionItemMigration : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToCollectionItemMigration()
        {
            Migrate(m =>
            {
                m.Description("Add new property to child item in document");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("CollectionMigrationPropertyAddition");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    foreach (JObject child in item["children"])
                    {
                        child.Add("addedDate", new JValue(DateTime.Now));
                    }
                    
                });
            });
        }
    }
}

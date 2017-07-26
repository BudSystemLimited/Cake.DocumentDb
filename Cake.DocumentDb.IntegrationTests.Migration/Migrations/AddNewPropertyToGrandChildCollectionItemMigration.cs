using System;
using System.Diagnostics;
using Cake.DocumentDb.Attributes;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201707241634)]
    public class AddNewPropertyToGrandChildCollectionItemMigration : DocumentDb.Migration.Migration
    {
        public AddNewPropertyToGrandChildCollectionItemMigration()
        {
            Migrate(m =>
            {
                m.Description("Add new property to child item in document");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("CollectionMigrationAddPropertyToGrandChild");
                m.PartitionKey("/mypartitionKey");
                m.Map((log, item) =>
                {
                    foreach (JObject child in item["children"])
                    {
                        foreach (JObject grandChild in child["grandChildren"])
                        {
                            grandChild.Add("addedDate", new JValue(DateTime.Now));
                        }
                    }
                    
                });
            });
        }
    }
}

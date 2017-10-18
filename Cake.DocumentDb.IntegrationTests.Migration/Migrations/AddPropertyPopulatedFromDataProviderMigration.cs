using System.Collections.Generic;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201710181606)]
    public class AddPropertyPopulatedFromDataProviderMigration : DataMigration
    {
        public AddPropertyPopulatedFromDataProviderMigration()
        {
            Migrate(m =>
            {
                m.Description("Add property populated from arbitrary data");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("DataMigration");
                m.PartitionKey("/mypartitionKey");
                m.DataProvider((log, settings) => new Dictionary<string, IList<JObject>>
                {
                    {
                        "dataset1", new List<JObject>
                        {
                            JObject.FromObject(new
                            {
                                Id = 1,
                                Age = 10
                            }),
                            JObject.FromObject(new
                            {
                                Id = 2,
                                Age = 20
                            })
                        }
                    }
                });
                m.Map((log, item, data) =>
                {
                    var record = data["dataset1"].Single(r => r["Id"].ToString() == item["id"].ToString());

                    item["age"] = record["Age"];
                });
            });
        }
    }
}
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201908301500)]
    public class PopulateFromDocumentDbWithQuery : DocumentHydration
    {
        public PopulateFromDocumentDbWithQuery()
        {
            Migrate(m =>
            {
                m.Description("Populate from document db with query");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyDocumentCollection");
                m.PartitionKey("/mypartitionKey");
                m.DocumentStatement(new DocumentStatement
                {
                    DatabaseName = "cakeddbhydrationtest",
                    CollectionName = "MigrationSource",
                    Query = new QuerySpec(
                        "select c.id, c.firstname from c where c.id = @id",
                        new
                        {
                            id = "1"
                        })

                });
                m.DocumentCreator((log, item, data) => JObject.FromObject(new
                {
                    id = $"C{item["id"].ToString()}",
                    mypartitionKey = "1",
                    firstname = item["firstname"]
                }));
            });
        }
    }
}
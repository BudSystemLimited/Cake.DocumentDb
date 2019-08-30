using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201708081640)]
    public class PopulateFromDocumentDb : DocumentHydration
    {
        public PopulateFromDocumentDb()
        {
            Migrate(m =>
            {
                m.Description("Add new document from a document collection");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyDocumentCollection");
                m.PartitionKey("/mypartitionKey");
                m.DocumentStatement(new DocumentStatement { DatabaseName = "cakeddbhydrationtest", CollectionName = "MigrationSource" });
                m.DocumentCreator((log, item, data) => JObject.FromObject(new
                {
                    id = $"A{item["id"].ToString()}",
                    mypartitionKey = item["mypartitionKey"],
                    firstname = item["firstname"]
                }));
            });
        }
    }
}

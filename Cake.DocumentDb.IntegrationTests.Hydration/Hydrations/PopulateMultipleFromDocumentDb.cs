using System.Linq;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201709111524)]
    public class PopulateMultipleFromDocumentDb : DocumentHydration
    {
        public PopulateMultipleFromDocumentDb()
        {
            Migrate(m =>
            {
                m.Description("Add multiple new document from a document collection");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyDocumentCollection");
                m.PartitionKey("/mypartitionKey");
                m.DocumentStatement(new DocumentStatement { DatabaseName = "cakeddbhydrationtest", CollectionName = "MigrationSource" });
                m.DocumentsCreator((log, item, data) =>
                {
                    return Enumerable.Range(0, 3)
                        .Select(i => JObject
                            .FromObject(new
                            {
                                id = $"M{item["id"].Value<int>() + i}",
                                mypartitionKey = item["mypartitionKey"],
                                firstname = item["firstname"]
                            }))
                        .ToList();
                });
            });
        }
    }
}

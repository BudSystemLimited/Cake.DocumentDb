using System.Linq;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201908301501)]
    public class PopulateFromDocumentDbWithQueryAndAdditionalDocumentStatements : DocumentHydration
    {
        public PopulateFromDocumentDbWithQueryAndAdditionalDocumentStatements()
        {
            Migrate(m =>
            {
                m.Description("Populate from document db with query and additional document statements");
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
                            id = "2"
                        })
                });
                m.AdditionalDocumentStatements(new []
                {
                    new DocumentStatement
                    {
                        AccessKey = "Additional",
                        DatabaseName = "cakeddbhydrationtest",
                        CollectionName = "MigrationSource",
                        Query = new QuerySpec(
                            "select c.id, c.surname from c where c.id = @id",
                            new
                            {
                                id = "2"
                            })
                    }
                });
                m.DocumentCreator((log, item, data) =>
                {
                    var id = item["id"].ToString();
                    var additional = data["Additional"].Single(d => d["id"].ToString() == id);

                    return JObject.FromObject(new
                    {
                        id = $"D{id}",
                        mypartitionKey = "1",
                        firstname = item["firstname"],
                        surname = additional["surname"]
                    });
                });
            });
        }
    }
}
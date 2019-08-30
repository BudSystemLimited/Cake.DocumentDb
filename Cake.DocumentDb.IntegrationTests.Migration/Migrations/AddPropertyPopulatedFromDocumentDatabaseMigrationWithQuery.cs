using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201908301502)]
    public class AddPropertyPopulatedFromDocumentDatabaseMigrationWithQuery : DocumentMigration
    {
        public AddPropertyPopulatedFromDocumentDatabaseMigrationWithQuery()
        {
            Migrate(m =>
            {
                m.Description("Add property populated from another document collection with query");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("DocumentMigrationOne");
                m.PartitionKey("/mypartitionKey");
                m.DocumentStatements(new[]
                {
                    new DocumentStatement
                    {
                        AccessKey = "DocumentMigrationTwo",
                        DatabaseName = "cakeddbmigrationtest",
                        CollectionName = "DocumentMigrationTwo",
                        Query = new QuerySpec(
                            "select c.id, c.mobile from c where c.id = @id",
                            new
                            {
                                id = "1"
                            })
                    }
                });
                m.Map((log, item, data) =>
                {
                    log.Write(Verbosity.Normal, LogLevel.Information, $"Trying to find record with id: {item["id"].ToString()}");

                    var record = data["DocumentMigrationTwo"].SingleOrDefault(r => r["id"].ToString() == item["id"].ToString());

                    if (record == null)
                        return;

                    log.Write(Verbosity.Normal, LogLevel.Information, $"Found record with id: {item["id"].ToString()}");

                    item["mobile"] = record["mobile"];
                    item["prop2"] = "AddPropertyPopulatedFromDocumentDatabaseMigrationWithQuery";
                });
            });
        }
    }
}
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Profile("local")]
    [Migration(201706121140)]
    public class AddPropertyPopulatedFromSqlDatabaseMigrationWithLocalProfile : SqlMigration
    {
        public AddPropertyPopulatedFromSqlDatabaseMigrationWithLocalProfile()
        {
            Migrate(m =>
            {
                m.Description("Add property populated from sql local");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.SqlStatements(new[]
                {
                    new SqlStatement {DataSource = "MyDataSourceOne", Statement = "SELECT * FROM Records"}
                });
                m.Map((log, item, data) =>
                {
                    log.Write(Verbosity.Normal, LogLevel.Information, $"Trying to find record with id: {item.id.ToString()}");

                    var record = data["MyDataSourceOne"].SingleOrDefault(r => r.Id.ToString() == item.id.ToString());

                    if (record == null)
                        return;

                    log.Write(Verbosity.Normal, LogLevel.Information, $"Found record with id: {item.id.ToString()}");
                    log.Write(Verbosity.Normal, LogLevel.Information, $"Adding property sqlPopulatedPropertyLocal with value : {record.LocalTitle}");

                    item.sqlPopulatedPropertyLocal = record.LocalTitle;
                });
            });
        }
    }
}
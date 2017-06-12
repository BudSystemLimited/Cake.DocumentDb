using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201706121140)]
    public class AddPropertyPopulatedFromSqlDatabaseMigration : SqlDocumentMigration
    {
        public override string Description => "Add property populated from sql";
        public override string DatabaseName => "cakeddbmigrationtest";
        public override string CollectionName => "MyCollection";
        public override string PartitionKey => "/mypartitionKey";

        public override SqlStatement[] SqlStatements => new[]
        {
            new SqlStatement { DataSource = "MyDataSourceOne", Statement = "SELECT * FROM Records" }
        };

        public override void Transform(dynamic item)
        {
            Log.Write(Verbosity.Normal, LogLevel.Information, $"Trying to find record with id: {item.id.ToString()}");

            var record = Data["MyDataSourceOne"].SingleOrDefault(r => r.Id.ToString() == item.id.ToString());

            if (record == null)
                return;

            Log.Write(Verbosity.Normal, LogLevel.Information, $"Found record with id: {item.id.ToString()}");
            Log.Write(Verbosity.Normal, LogLevel.Information, $"Adding property sqlPopulatedProperty with value : {record.Title}");
            item.sqlPopulatedProperty = record.Title;
        }
    }
}

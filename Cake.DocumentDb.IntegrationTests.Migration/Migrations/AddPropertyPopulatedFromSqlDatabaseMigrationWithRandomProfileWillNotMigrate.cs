using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Profile("random")]
    [Migration(201706121140)]
    public class AddPropertyPopulatedFromSqlDatabaseMigrationWithRandomProfileWillNotMigrate : SqlDocumentMigration
    {
        public override string Description => "Add property populated from sql random";
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
            Log.Write(Verbosity.Normal, LogLevel.Information, $"Adding property sqlPopulatedPropertyLocal with value : {record.RandomTitle}");
            item.sqlPopulatedPropertyLocal = record.RandomTitle;
        }
    }
}
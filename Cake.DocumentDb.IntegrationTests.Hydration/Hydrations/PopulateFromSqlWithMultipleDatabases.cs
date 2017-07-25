using System.Diagnostics;
using System.Linq;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201707191414)]
    public class PopulateFromSqlWithMultipleDatabases : SqlHydration
    {
        public PopulateFromSqlWithMultipleDatabases()
        {
            Migrate(m =>
            {
                m.Description("Add new document from sql using multiple databases");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.SqlStatement(new SqlStatement { DataSource = "MyDataSourceOne", Statement = "SELECT * FROM Records" });
                m.AdditionalSqlStatements(new []
                {
                    new SqlStatement { DataSource = "MyDataSourceTwo", Statement = "SELECT * FROM OtherRecords" }
                });
                m.DocumentCreator((log, item, data) =>
                {
                    var row = data["MyDataSourceTwo"].SingleOrDefault(x => x.RecordId == item.Id);

                    var record = new
                    {
                        id = (item.Id * 100).ToString(),
                        title = item.Title,
                        localTitle = item.LocalTitle,
                        randomTitle = item.RandomTitle,
                        mypartitionKey = item.Id,
                        myPropFromOtherDb = row != null ? row.Total : null
                    };

                    return record;
                });
            });
        }
    }
}

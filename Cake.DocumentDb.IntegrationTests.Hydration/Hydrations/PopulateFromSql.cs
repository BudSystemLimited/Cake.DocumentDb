using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201707180905)]
    public class PopulateFromSql : SqlHydration
    {
        public PopulateFromSql()
        {
            Migrate(m =>
            {
                m.Description("Add new document from sql");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.SqlStatement(new SqlStatement { DataSource = "MyDataSourceOne", Statement = "SELECT * FROM Records" });
                m.DocumentCreator((log, item) => new
                {
                    id = item.Id.ToString(),
                    title = item.Title,
                    localTitle = item.LocalTitle,
                    randomTitle = item.RandomTitle,
                    mypartitionKey = item.Id
                });
            });
        }
    }
}

﻿using System.Diagnostics;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201706121140)]
    public class AddPropertyPopulatedFromSqlDatabaseMigration : SqlMigration
    {
        public AddPropertyPopulatedFromSqlDatabaseMigration()
        {
            Migrate(m =>
            {
                m.Description("Add property populated from sql");
                m.DatabaseName("cakeddbmigrationtest");
                m.CollectionName("MyCollection");
                m.PartitionKey("/mypartitionKey");
                m.SqlStatements(new[]
                {
                    new SqlStatement { DataSource = "MyDataSourceOne", Statement = "SELECT * FROM Records" }
                });
                m.Map((log, item, data) =>
                {
                    log.Write(Verbosity.Normal, LogLevel.Information, $"Trying to find record with id: {item["id"].ToString()}");

                    var record = data["MyDataSourceOne"].SingleOrDefault(r => r.Id.ToString() == item["id"].Value<string>());

                    if (record == null)
                        return;

                    log.Write(Verbosity.Normal, LogLevel.Information, $"Found record with id: {item["id"].ToString()}");
                    log.Write(Verbosity.Normal, LogLevel.Information, $"Adding property sqlPopulatedProperty with value : {record.Title}");

                    item.Add("sqlPopulatedProperty", new JValue(record.Title));
                });
            });
        }
    }
}

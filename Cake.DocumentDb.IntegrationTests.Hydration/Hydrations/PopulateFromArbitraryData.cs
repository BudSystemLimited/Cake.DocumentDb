using System;
using System.Collections.Generic;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;

namespace Cake.DocumentDb.IntegrationTests.Hydration.Hydrations
{
    [Migration(201708091455)]
    public class PopulateFromArbitraryData : DataHydration
    {
        public PopulateFromArbitraryData()
        {
            Migrate(m =>
            {
                m.Description("Add new document from arbitrary data");
                m.DatabaseName("cakeddbhydrationtest");
                m.CollectionName("MyDataCollection");
                m.PartitionKey("/mypartitionkey");
                m.DataProvider(settings => new List<dynamic>
                {
                    new
                    {
                        Id = Guid.NewGuid(),
                        Name = "Some Guy",
                        Age = 21,
                        Location = "England"
                    },
                    new
                    {
                        Id = Guid.NewGuid(),
                        Name = "Some Gal",
                        Age = 23,
                        Location = "France"
                    }
                });
                m.DocumentCreator((log, item) =>
                {
                    var record = new
                    {
                        id = item.Id.ToString(),
                        name = item.Name,
                        age = item.Age,
                        mypartitionkey = item.Location
                    };

                    return record;
                });
            });
        }
    }
}

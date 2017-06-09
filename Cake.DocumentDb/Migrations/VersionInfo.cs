using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Cake.DocumentDb.Migrations
{
    public class VersionInfo : Document
    {
        [JsonProperty(PropertyName = "processedMigrations")]
        public IList<MigrationInfo> ProcessedMigrations { get; set; }

        public VersionInfo()
        {
            ProcessedMigrations = new List<MigrationInfo>();
        }
    }
}

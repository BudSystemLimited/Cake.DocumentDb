using System;
using Newtonsoft.Json;

namespace Cake.DocumentDb.Migration
{
    public class MigrationInfo
    {
        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "appliedOn")]
        public DateTime AppliedOn { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}

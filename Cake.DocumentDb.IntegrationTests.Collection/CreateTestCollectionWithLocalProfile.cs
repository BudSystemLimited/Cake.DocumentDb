using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Collection
{
    [Profile("local")]
    public class CreateTestCollectionWithLocalProfile : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbcoltest";
        public string CollectionName => "MyCollectionLocal";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
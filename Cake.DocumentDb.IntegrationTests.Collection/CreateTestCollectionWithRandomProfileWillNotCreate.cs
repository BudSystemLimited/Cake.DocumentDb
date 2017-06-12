using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Collection
{
    [Profile("random")]
    public class CreateTestCollectionWithRandomProfileWillNotCreate : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbcoltest";
        public string CollectionName => "MyCollectionRandom";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}
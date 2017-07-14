using Cake.DocumentDb.Collection;

namespace Cake.DocumentDb.IntegrationTests.Deletions
{
    public class CreateTestCollection : ICreateDocumentDatabaseCollection
    {
        public string DatabaseName => "cakeddbdeletiontest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public int? Throughput => 2000;
    }
}

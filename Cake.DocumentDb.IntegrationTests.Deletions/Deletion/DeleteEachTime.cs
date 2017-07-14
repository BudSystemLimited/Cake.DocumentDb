using Cake.DocumentDb.Deletion;

namespace Cake.DocumentDb.IntegrationTests.Deletions.Deletion
{
    public class DeleteEachTime : DeleteDocuments
    {
        public DeleteEachTime()
        {
            Delete(d =>
            {
                d.Description("Delete Each Time");
                d.DatabaseName("cakeddbdeletiontest");
                d.CollectionName("MyCollection");
                d.PartitionKey("/mypartitionKey");
                d.PartitionKeyAccessor(doc => doc.mypartitionKey);
                d.Filter(doc => doc.mypartitionKey == "1");
            });
        }
    }
}

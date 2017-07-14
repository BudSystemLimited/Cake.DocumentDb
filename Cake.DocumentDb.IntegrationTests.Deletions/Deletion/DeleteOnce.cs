using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Deletion;

namespace Cake.DocumentDb.IntegrationTests.Deletions.Deletion
{
    [Migration(201707141437)]
    public class DeleteOnce : DeleteDocuments
    {
        public DeleteOnce()
        {
            Delete(d =>
            {
                d.Description("Delete Once");
                d.DatabaseName("cakeddbdeletiontest");
                d.CollectionName("MyCollection");
                d.PartitionKey("/mypartitionKey");
                d.PartitionKeyAccessor(doc => doc.mypartitionKey);
                d.Filter(doc => doc.mypartitionKey == "2");
            });
        }
    }
}

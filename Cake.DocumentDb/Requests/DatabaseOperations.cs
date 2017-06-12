using System.Linq;
using Cake.Core;
using Cake.DocumentDb.Factories;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;

namespace Cake.DocumentDb.Requests
{
    public class DatabaseOperations
    {
        private readonly IReliableReadWriteDocumentClient client;

        public DatabaseOperations(ConnectionSettings settings, ICakeContext context)
            :this(new ClientFactory(settings, context))
        { }

        public DatabaseOperations(ClientFactory clientFactory)
        {
            this.client = clientFactory.GetClient();
        }

        public Database GetOrCreateDatabaseIfNotExists(string database)
        {
            var response = client.CreateDatabaseQuery()
                 .Where(db => db.Id == database)
                 .ToArray()
                 .FirstOrDefault();

            if (response != null)
                return response;

            response = client.CreateDatabaseAsync(
                new Database
                {
                    Id = database
                }).Result;

            return response;
        }
    }
}

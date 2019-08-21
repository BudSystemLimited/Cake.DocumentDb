using System.Linq;
using Cake.Core;
using Cake.DocumentDb.Factories;
using Microsoft.Azure.Documents;

namespace Cake.DocumentDb.Requests
{
    public class DatabaseOperations
    {
        private readonly IDocumentClient client;

        public DatabaseOperations(ConnectionSettings settings, ICakeContext context)
            :this(new ClientFactory(settings, context))
        { }

        public DatabaseOperations(ClientFactory clientFactory)
        {
            this.client = clientFactory.GetClient();
        }

        public Microsoft.Azure.Documents.Database GetOrCreateDatabaseIfNotExists(string database)
        {
            var response = client.CreateDatabaseQuery()
                 .Where(db => db.Id == database)
                 .ToArray()
                 .FirstOrDefault();

            if (response != null)
                return response;

            response = client.CreateDatabaseAsync(
                new Microsoft.Azure.Documents.Database
                {
                    Id = database
                }).Result;

            return response;
        }
    }
}

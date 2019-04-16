using System;
using Cake.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Cake.DocumentDb.Factories
{
    public class ClientFactory
    {
        private readonly ConnectionSettings settings;
        private readonly ICakeContext context;

        public ClientFactory(ConnectionSettings settings, ICakeContext context)
        {
            this.settings = settings;
            this.context = context;
        }

        public IReliableReadWriteDocumentClient GetClient()
        {
            return new DocumentClient(
                new Uri(settings.Endpoint),
                settings.Key,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    ConnectionProtocol = Protocol.Https
                })
                .AsReliable(
                    new FixedInterval(
                        15,
                        TimeSpan.FromSeconds(200)));
        }

        // https://github.com/Azure/azure-cosmos-dotnet-v2/blob/master/samples/documentdb-benchmark/Program.cs
        public IDocumentClient GetClientOptimisedForWrite()
        {
            return new DocumentClient(
                new Uri(settings.Endpoint),
                settings.Key,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    RequestTimeout = new TimeSpan(1, 0, 0),
                    MaxConnectionLimit = 1000,
                    RetryOptions = new RetryOptions
                    {
                        MaxRetryAttemptsOnThrottledRequests = 10,
                        MaxRetryWaitTimeInSeconds = 60
                    }
                });
        }
    }
}

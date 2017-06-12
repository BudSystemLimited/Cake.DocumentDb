using System;
using Cake.Core;
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
                }).AsReliable(new FixedInterval(
                    15,
                    TimeSpan.FromSeconds(200)));
        }
    }
}

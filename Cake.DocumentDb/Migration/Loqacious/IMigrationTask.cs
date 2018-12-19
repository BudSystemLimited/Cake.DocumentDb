using System;
using System.Linq.Expressions;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal interface IMigrationTask
    {
        string Description { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
        string PartitionKey { get; }
        Expression<Func<Document, bool>> Filter { get; }
    }
}

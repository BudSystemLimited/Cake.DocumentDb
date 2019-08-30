using System;
using Microsoft.Azure.Documents;

namespace Cake.DocumentDb.Migration
{
    public class DocumentStatement
    {
        public string AccessKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        /// <summary>
        /// Allows custom Cosmos query definition, including selecting a subset of fields. Use either <see cref="Query"/>, <see cref="Filter"/> or neither.
        /// </summary>
        public SqlQuerySpec Query { get; set; }
        /// <summary>
        /// Allows filtering of the documents returned from Cosmos. Use either <see cref="Query"/>, <see cref="Filter"/> or neither.
        /// </summary>
        public Func<dynamic, bool> Filter { get; set; }
    }
}
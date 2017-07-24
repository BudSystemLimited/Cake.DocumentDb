using System;

namespace Cake.DocumentDb.Migration
{
    public class DocumentStatement
    {
        public string AccessKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public Func<dynamic, bool> Filter { get; set; }
    }
}
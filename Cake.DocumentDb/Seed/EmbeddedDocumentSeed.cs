using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Cake.DocumentDb.Seed
{
    public abstract class EmbeddedDocumentSeed : ISeedDocument
    {
        public abstract string FriendlyName { get; }
        public abstract string Database { get; }
        public abstract string Collection { get; }
        public abstract string PartitionKey { get; }
        public abstract string DocumentName { get; }
        public object Document()
        {
            using (var stream = Assembly.GetAssembly(GetType()).GetManifestResourceStream(
                GetType(),
                DocumentName))
            {
                if (stream == null)
                    throw new NotSupportedException($"Could not load document");

                using (var reader = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject(reader.ReadToEnd());
                }
            }
        }
    }
}

using System;

namespace Cake.DocumentDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute : Attribute
    {
        public long Timestamp { get; set; }

        public MigrationAttribute(long timestamp)
        {
            Timestamp = timestamp;
        }
    }
}

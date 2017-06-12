using System;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;

namespace Cake.DocumentDb.IntegrationTests.Migration.Migrations
{
    [Migration(201706121107)]
    public class AddNewPropertyToDocumentMigration : IDocumentMigration
    {
        public ICakeLog Log { get; set; }
        public string Description => "Add new property to document";
        public string DatabaseName => "cakeddbmigrationtest";
        public string CollectionName => "MyCollection";
        public string PartitionKey => "/mypartitionKey";
        public void Transform(dynamic item)
        {
            item.myNewStringProperty = "my new value";
            item.myNewIntProperty = 1;
            item.myNewBoolProperty = true;
            item.myNewGuidProperty = Guid.NewGuid();
        }
    }
}

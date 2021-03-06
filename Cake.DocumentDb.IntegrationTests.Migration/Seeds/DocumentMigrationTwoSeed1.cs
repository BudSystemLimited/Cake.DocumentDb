﻿using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.IntegrationTests.Migration.Seeds
{
    public class DocumentMigrationTwoSeed1 : EmbeddedDocumentSeed
    {
        public override string FriendlyName => "Document Migration Two Test";
        public override string Database => "cakeddbmigrationtest";
        public override string Collection => "DocumentMigrationTwo";
        public override string PartitionKey => "/mypartitionKey";
        public override string DocumentName => "documentmigrationtwo1.json";
    }
}

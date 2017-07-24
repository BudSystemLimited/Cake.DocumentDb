﻿using System;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class MigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, dynamic> map;
        private Func<dynamic, bool> filter;

        public void Description(string setDescription)
        {
            description = setDescription;
        }

        public void DatabaseName(string setDatabaseName)
        {
            databaseName = setDatabaseName;
        }

        public void CollectionName(string setCollectionName)
        {
            collectionName = setCollectionName;
        }

        public void PartitionKey(string setPartitionKey)
        {
            partitionKey = setPartitionKey;
        }

        public void Map(Action<ICakeLog, dynamic> setMap)
        {
            map = setMap;
        }

        public void Filter(Func<dynamic, bool> setFilter)
        {
            filter = setFilter;
        }

        internal MigrationTask MigrationTask => new MigrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            map,
            filter);
    }
}
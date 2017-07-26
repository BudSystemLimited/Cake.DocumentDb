using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Deletion.Loqacious;

namespace Cake.DocumentDb.Deletion
{
    public class DeleteDocuments
    {
        internal MigrationAttribute Attribute { get; set; }

        internal DeletionTask Task { get; private set; }

        protected void Delete(Action<DeletionConfigurationCreator> action)
        {
            var creator = new DeletionConfigurationCreator();

            action(creator);

            Task = creator.DeletionTask;
        }
    }
}
using System;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Extensions
{
    public static class CakeContextExtensions
    {
        public static void LogExceptionHierarchyAsErrors(this ICakeContext context, Exception exception)
        {
            if (exception == null)
                throw new ArgumentException("Cannot be null", nameof(exception));

            context.Log.Error(exception.Message);
            context.Log.Error(exception.StackTrace);

            if (exception.InnerException != null)
                LogExceptionHierarchyAsErrors(context, exception.InnerException);
        }
    }
}

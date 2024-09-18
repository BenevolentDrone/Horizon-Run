using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Entities.Factories
{
    public static class EntityFactory
    {
        public static ManagedTypeResourceManager<TResource, THandle> BuildManagedTypeResourceManager<TResource, THandle>(
            Func<THandle, THandle> newHandleAllocationDelegate,
            Func<TResource> newResourceAllocationDelegate,
            ILoggerResolver loggerResolver = null)
        {
            return new ManagedTypeResourceManager<TResource, THandle>(
                RepositoriesFactory.BuildDictionaryRepository<THandle, TResource>(),
				new Queue<THandle>(),
                newHandleAllocationDelegate,
                newResourceAllocationDelegate,
				loggerResolver?.GetLogger<ManagedTypeResourceManager<TResource, THandle>>());
        }
    }
}
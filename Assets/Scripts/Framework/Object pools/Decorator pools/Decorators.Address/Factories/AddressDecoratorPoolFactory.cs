using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class AddressDecoratorPoolFactory 
    {
        public static ManagedPoolWithAddress<T> BuildPoolWithAddress<T>(
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithAddress<T>>()
                ?? null;

            return new ManagedPoolWithAddress<T>(
                RepositoriesFactory.BuildDictionaryRepository<int, IManagedPool<T>>(),
                0,
                new PoolWithAddressBuilder<T>(
                    loggerResolver,
                    loggerResolver?.GetLogger<PoolWithAddressBuilder<T>>()),
                logger);
        }
        
        public static ManagedPoolWithAddress<T> BuildPoolWithAddress<T>(
            IRepository<int, IManagedPool<T>> repository,
            int level,
            PoolWithAddressBuilder<T> builder = null,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithAddress<T>>()
                ?? null;

            return new ManagedPoolWithAddress<T>(
                repository,
                level,
                builder,
                logger);
        }
    }
}
using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class AddressDecoratorPoolFactory 
    {
        public static ManagedPoolWithAddress<T> BuildPoolWithAddress<T>(
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithAddress<T>>();

            return new ManagedPoolWithAddress<T>(
                RepositoryFactory.BuildDictionaryRepository<int, IManagedPool<T>>(),
                0,
                logger,
                new PoolWithAddressBuilder<T>(
                    loggerResolver,
                    loggerResolver?.GetLogger<PoolWithAddressBuilder<T>>()));
        }
        
        public static ManagedPoolWithAddress<T> BuildPoolWithAddress<T>(
            IRepository<int, IManagedPool<T>> repository,
            int level,
            ILoggerResolver loggerResolver,
            PoolWithAddressBuilder<T> builder = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithAddress<T>>();

            return new ManagedPoolWithAddress<T>(
                repository,
                level,
                logger,
                builder);
        }
    }
}
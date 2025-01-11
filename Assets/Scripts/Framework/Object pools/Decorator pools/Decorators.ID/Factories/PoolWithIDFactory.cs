using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class PoolWithIDFactory
    {
        public static PoolWithID<T> BuildPoolWithID<T>(
            IPool<T> innerPool,
            string id)
        {
            return new PoolWithID<T>(innerPool, id);
        }

        public static ManagedPoolWithID<T> BuildManagedPoolWithID<T>(
            IManagedPool<T> innerPool,
            string id,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithID<T>>();

            return new ManagedPoolWithID<T>(
                innerPool,
                id,
                logger);
        }
    }
}
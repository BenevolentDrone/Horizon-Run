using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class VariantDecoratorPoolFactory
    {
        
        public static ManagedPoolWithVariants<T> BuildPoolWithVariants<T>(
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithVariants<T>>();

            return new ManagedPoolWithVariants<T>(
                RepositoryFactory.BuildDictionaryRepository<int, VariantContainer<T>>(),
                RandomFactory.BuildSystemRandomGenerator(),
                logger);
        }
        
        public static ManagedPoolWithVariants<T> BuildPoolWithVariants<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithVariants<T>>();

            return new ManagedPoolWithVariants<T>(
                repository,
                generator,
                logger);
        }
    }
}
using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Factory class for creating pools with decorators.
    /// </summary>
    public static class VariantDecoratorPoolsFactory
    {
        
        public static ManagedPoolWithVariants<T> BuildPoolWithVariants<T>(
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithVariants<T>>()
                ?? null;

            return new ManagedPoolWithVariants<T>(
                RepositoriesFactory.BuildDictionaryRepository<int, VariantContainer<T>>(),
                RandomFactory.BuildSystemRandomGenerator(),
                logger);
        }
        
        public static ManagedPoolWithVariants<T> BuildPoolWithVariants<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithVariants<T>>()
                ?? null;

            return new ManagedPoolWithVariants<T>(
                repository,
                generator,
                logger);
        }
    }
}
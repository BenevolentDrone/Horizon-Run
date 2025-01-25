using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class VariantDecoratorAllocationCallbackFactory
    {
        public static SetVariantCallback<T> BuildSetVariantCallback<T>(
            ILoggerResolver loggerResolver,
            int variant = -1)
        {
            ILogger logger = loggerResolver?.GetLogger<SetVariantCallback<T>>();

            return new SetVariantCallback<T>(
                logger,
                variant);
        }
    }
}
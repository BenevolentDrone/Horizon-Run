using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating set variant callbacks for variant decorators pools.
    /// </summary>
    public static class VariantDecoratorAllocationCallbacksFactory
    {
        public static SetVariantCallback<T> BuildSetVariantCallback<T>(int variant = -1)
        {
            return new SetVariantCallback<T>(variant);
        }
    }
}
using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static class VariantDecoratorAllocationCallbackFactory
    {
        public static SetVariantCallback<T> BuildSetVariantCallback<T>(int variant = -1)
        {
            return new SetVariantCallback<T>(variant);
        }
    }
}
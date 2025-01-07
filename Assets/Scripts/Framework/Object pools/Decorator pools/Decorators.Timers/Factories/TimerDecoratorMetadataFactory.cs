using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Factories
{
    public static class TimerDecoratorMetadataFactory
    {
        public static RuntimeTimerWithPushableSubscriptionMetadata BuildRuntimeTimerWithPushSubscriptionMetadata()
        {
            return new RuntimeTimerWithPushableSubscriptionMetadata();
        }

        public static MetadataAllocationDescriptor BuildRuntimeTimerWithPushSubscriptionMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsRuntimeTimer),
                ConcreteType = typeof(RuntimeTimerWithPushableSubscriptionMetadata)
            };
        }
    }
}
using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory for creating timer metadata objects.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Metadata

        public static RuntimeTimerWithPushSubscriptionMetadata BuildRuntimeTimerWithPushSubscriptionMetadata()
        {
            return new RuntimeTimerWithPushSubscriptionMetadata();
        }

        public static MetadataAllocationDescriptor BuildRuntimeTimerWithPushSubscriptionMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsRuntimeTimer),
                ConcreteType = typeof(RuntimeTimerWithPushSubscriptionMetadata)
            };
        }

        #endregion
    }
}
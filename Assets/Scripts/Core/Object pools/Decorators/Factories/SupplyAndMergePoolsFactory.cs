using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.GenericNonAlloc;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class PoolsFactory
    {
        #region Supply and merge pool

        public static SupplyAndMergePool<T> BuildSupplyAndMergePoolWithAllocationCallback<T>(
            Func<T> valueAllocationDelegate,
            bool topUpIfElementValueIsNull,

            MetadataAllocationDescriptor[] metadataDescriptors,

            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,

            IAllocationCallback<T> callback,

            ILoggerResolver loggerResolver = null)
        {
            Func<T> nullAllocation = AllocationsFactory.NullAllocationDelegate<T>;

            SupplyAndMergePool<T> supplyAndMergePool = BuildSupplyAndMergePool<T>(
                BuildPoolElementAllocationCommandWithCallback<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors,
                    callback),
                BuildPoolElementAllocationCommandWithCallback<T>(
                    additionalAllocation,
                    nullAllocation,
                    metadataDescriptors,
                    callback),

                valueAllocationDelegate,
                topUpIfElementValueIsNull,

                loggerResolver);

            return supplyAndMergePool;
        }

        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            Func<T> valueAllocationDelegate,
            bool topUpIfElementValueIsNull,


            MetadataAllocationDescriptor[] metadataDescriptors,

            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,

            ILoggerResolver loggerResolver = null)
        {
            Func<T> nullAllocation = AllocationsFactory.NullAllocationDelegate<T>;

            SupplyAndMergePool<T> supplyAndMergePool = BuildSupplyAndMergePool<T>(
                BuildPoolElementAllocationCommand<T>(
                    initialAllocation,
                    valueAllocationDelegate,
                    metadataDescriptors),
                BuildPoolElementAllocationCommand<T>(
                    additionalAllocation,
                    nullAllocation,
                    metadataDescriptors),

                valueAllocationDelegate,
                topUpIfElementValueIsNull,

                loggerResolver);

            return supplyAndMergePool;
        }

        public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
            AllocationCommand<IPoolElement<T>> initialAllocationCommand,
            AllocationCommand<IPoolElement<T>> appendAllocationCommand,

            Func<T> topUpAllocationDelegate,
            bool topUpIfElementValueIsNull,

            ILoggerResolver loggerResolver = null)
        {
            var basePool = BuildPackedArrayPool<T>(
                initialAllocationCommand,
                loggerResolver);

            //¯\_(ツ)_/¯
            //TODO: fix
            if (appendAllocationCommand.Descriptor.Rule == EAllocationAmountRule.DOUBLE_AMOUNT)
            {
                throw new Exception($"[SupplyAndMergePoolsFactory] CANNOT USE \"DOUBLE_AMOUNT\" RULE FOR APPEND ALLOCATION COMMAND IN SUPPLY AND MERGE POOL");
            }

            var supplyPool = BuildPackedArrayPool<T>(
                appendAllocationCommand,
                loggerResolver);

            ILogger logger =
                loggerResolver?.GetLogger<SupplyAndMergePool<T>>()
                ?? null;

            return new SupplyAndMergePool<T>(
                basePool,
                supplyPool,

                supplyPool,
                supplyPool,

                appendAllocationCommand,
                MergePools,

                topUpAllocationDelegate,
                topUpIfElementValueIsNull,

                logger);
        }

        public static void MergePools<T>(
            INonAllocPool<T> receiverArray,
            INonAllocPool<T> donorArray,
            AllocationCommand<IPoolElement<T>> donorAllocationCommand)
        {
            MergePackedArrayPools(
                (PackedArrayPool<T>)receiverArray,
                (PackedArrayPool<T>)donorArray,
                donorAllocationCommand);
        }

        #endregion
    }
}
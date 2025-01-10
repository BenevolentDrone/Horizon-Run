using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class PackedArrayPoolFactory
    {
        #region Packed array pool

        #region Build

        #region PackedArrayPool

        public static PackedArrayPool<T> BuildPackedArrayPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<PackedArrayPool<T>>()
                ?? null;

            int initialAmount = CountInitialAllocationAmount(
                initialAllocationCommand.Descriptor);

            T[] contents = new T[initialAmount];

            PerformInitialAllocation<T>(
                initialAmount,
                contents,
                initialAllocationCommand);

            return new PackedArrayPool<T>(
                contents,
                additionalAllocationCommand,
                logger);
        }

        private static int CountInitialAllocationAmount(
            AllocationCommandDescriptor descriptor)
        {
            int initialAmount = -1;

            switch (descriptor.Rule)
            {
                case EAllocationAmountRule.ZERO:
                    initialAmount = 0;
                    break;

                case EAllocationAmountRule.ADD_ONE:
                    initialAmount = 1;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    initialAmount = descriptor.Amount;
                    break;

                default:
                    throw new Exception($"[PackedArrayPoolFactory] INVALID ALLOCATION COMMAND RULE: {descriptor.Rule.ToString()}");
            }

            return initialAmount;
        }

        private static void PerformInitialAllocation<T>(
            int initialAmount,
            T[] contents,
            AllocationCommand<T> allocationCommand)
        {
            for (int i = 0; i < initialAmount; i++)
            {
                var newElement = allocationCommand.AllocationDelegate();
                
                allocationCommand.AllocationCallback?.OnAllocated(
                    newElement);

                contents[i] = newElement;
            }
        }
        
        #endregion

        #region PackedArrayManagedPool

        public static PackedArrayManagedPool<T> BuildPackedArrayManagedPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null,
            bool validateValues = true,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<PackedArrayManagedPool<T>>()
                ?? null;
            
            Func<IPoolElementFacade<T>> facadeAllocationDelegate = 
                () => ObjectPoolAllocationFactory.BuildPoolElementFacadeWithArrayIndex<T>(
                    metadataAllocationDescriptors);

            AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
                ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
                    initialAllocationCommand.Descriptor,
                    facadeAllocationDelegate,
                    facadeAllocationCallback);
            
            AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
                ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
                    additionalAllocationCommand.Descriptor,
                    facadeAllocationDelegate,
                    facadeAllocationCallback);

            int initialAmount = CountInitialAllocationAmount(
                initialFacadeAllocationCommand.Descriptor);

            IPoolElementFacade<T>[] contents = new IPoolElementFacade<T>[initialAmount];

            PerformInitialAllocation<T>(
                initialAmount,
                contents,
                initialFacadeAllocationCommand,
                initialAllocationCommand);

            return new PackedArrayManagedPool<T>(
                contents,
                additionalFacadeAllocationCommand,
                additionalAllocationCommand,
                validateValues,
                logger);
        }
        
        private static void PerformInitialAllocation<T>(
            int initialAmount,
            IPoolElementFacade<T>[] contents,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand)
        {
            for (int i = 0; i < initialAmount; i++)
            {
                var newElement = facadeAllocationCommand.AllocationDelegate();

                //MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
                //facadeAllocationCommand.AllocationCallback?.OnAllocated(
                //    newElement);

                var newElementValue = valueAllocationCommand.AllocationDelegate();

                valueAllocationCommand.AllocationCallback?.OnAllocated(
                    newElementValue);

                newElement.Value = newElementValue;
                
                //THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
                newElement.Status = EPoolElementStatus.PUSHED;
                
                facadeAllocationCommand.AllocationCallback?.OnAllocated(
                    newElement);
                
                contents[i] = newElement;
            }
        }
        
        #endregion

        #region AppendableManagedPool
        
        public static AppendablePackedArrayManagedPool<T> BuildAppendableManagedPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AppendablePackedArrayManagedPool<T>>()
                ?? null;

            Func<IPoolElementFacade<T>> facadeAllocationDelegate = 
                () => ObjectPoolAllocationFactory.BuildPoolElementFacadeWithArrayIndex<T>(
                    metadataAllocationDescriptors);
            
            AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
                ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
                    initialAllocationCommand.Descriptor,
                    facadeAllocationDelegate,
                    facadeAllocationCallback);
            
            AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
                ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
                    additionalAllocationCommand.Descriptor,
                    facadeAllocationDelegate,
                    facadeAllocationCallback);
            
            AllocationCommand<T> nullValueAllocationCommand =
                new AllocationCommand<T>
                {
                    Descriptor = new AllocationCommandDescriptor
                    {
                        Rule = EAllocationAmountRule.ADD_ONE,
                        
                        Amount = 1
                    },
                    
                    AllocationDelegate = AllocationFactory.NullAllocationDelegate<T>
                };
            
            AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand =
                ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
                    nullValueAllocationCommand.Descriptor,
                    facadeAllocationDelegate,
                    facadeAllocationCallback);
            
            int initialAmount = CountInitialAllocationAmount(
                initialFacadeAllocationCommand.Descriptor);

            IPoolElementFacade<T>[] contents = new IPoolElementFacade<T>[initialAmount];

            PerformInitialAllocation<T>(
                initialAmount,
                contents,
                initialFacadeAllocationCommand,
                initialAllocationCommand);

            return new AppendablePackedArrayManagedPool<T>(
                contents,
                additionalFacadeAllocationCommand,
                additionalAllocationCommand,
                
                appendFacadeAllocationCommand,
                nullValueAllocationCommand,
                
                logger);
        }

        #endregion

        #endregion

        #region Resize

        public static T[] ResizePackedArrayPool<T>(
            T[] packedArray,
            AllocationCommand<T> allocationCommand,
            ILogger logger = null)
        {
            int newCapacity = CountResizeAllocationAmount<T>(
                packedArray,
                allocationCommand,
                logger);

            T[] newContents = new T[newCapacity];

            FillNewArrayWithContents<T>(
                packedArray,
                newContents,
                allocationCommand);

            return newContents;
        }

        private static int CountResizeAllocationAmount<T>(
            T[] packedArray,
            AllocationCommand<T> allocationCommand,
            ILogger logger = null)
        {
            int newCapacity = -1;

            switch (allocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ADD_ONE:
                    newCapacity = packedArray.Length + 1;
                    break;

                case EAllocationAmountRule.DOUBLE_AMOUNT:
                    newCapacity = Math.Max(packedArray.Length, 1) * 2;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    newCapacity = packedArray.Length + allocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception(
                        logger.TryFormatException(
                            $"[PackedArrayPoolFactory] INVALID ALLOCATION COMMAND RULE FOR PACKED ARRAY: {allocationCommand.Descriptor.Rule.ToString()}"));
            }

            return newCapacity;
        }

        private static void FillNewArrayWithContents<T>(
            T[] oldContents,
            T[] newContents,
            AllocationCommand<T> allocationCommand)
        {
            for (int i = 0; i < oldContents.Length; i++)
                newContents[i] = oldContents[i];

            for (int i = oldContents.Length; i < newContents.Length; i++)
            {
                var newElement = allocationCommand.AllocationDelegate();
                
                allocationCommand.AllocationCallback?.OnAllocated(
                    newElement);

                newContents[i] = newElement;
            }
        }
        
        public static IPoolElementFacade<T>[] ResizePackedArrayManagedPool<T>(
            IPoolElementFacade<T>[] packedArray,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            ILogger logger = null)
        {
            int newCapacity = CountResizeAllocationAmount<IPoolElementFacade<T>>(
                packedArray,
                facadeAllocationCommand,
                logger);

            IPoolElementFacade<T>[] newContents = new IPoolElementFacade<T>[newCapacity];

            FillNewArrayWithContents(
                packedArray,
                newContents,
                facadeAllocationCommand,
                valueAllocationCommand);

            return newContents;
        }

        private static void FillNewArrayWithContents<T>(
            IPoolElementFacade<T>[] oldContents,
            IPoolElementFacade<T>[] newContents,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand)
        {
            for (int i = 0; i < oldContents.Length; i++)
                newContents[i] = oldContents[i];

            for (int i = oldContents.Length; i < newContents.Length; i++)
            {
                var newElement = facadeAllocationCommand.AllocationDelegate();
                
                //MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
                //facadeAllocationCommand.AllocationCallback?.OnAllocated(
                //    newElement);

                var newElementValue = valueAllocationCommand.AllocationDelegate();
                    
                valueAllocationCommand.AllocationCallback?.OnAllocated(
                    newElementValue);

                newElement.Value = newElementValue;
                
                //THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
                newElement.Status = EPoolElementStatus.PUSHED;
                
                facadeAllocationCommand.AllocationCallback?.OnAllocated(
                    newElement);
                
                
                newContents[i] = newElement;
            }
        }
        
        #endregion

        #endregion
    }
}
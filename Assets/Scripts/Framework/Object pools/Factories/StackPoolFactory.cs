using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class StackPoolFactory
    {
        #region Build

        #region Stack pool

        public static StackPool<T> BuildStackPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<StackPool<T>>();
            
            var stack = new Stack<T>();

            PerformInitialAllocation<T>(
                stack,
                initialAllocationCommand,
                logger);

            return new StackPool<T>(
                stack,
                additionalAllocationCommand,
                logger);
        }
        
        private static void PerformInitialAllocation<T>(
            Stack<T> stack,
            AllocationCommand<T> initialAllocationCommand,
            ILogger logger)
        {
            int initialAmount = -1;

            switch (initialAllocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ZERO:
                    initialAmount = 0;
                    break;

                case EAllocationAmountRule.ADD_ONE:
                    initialAmount = 1;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    initialAmount = initialAllocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception(
                        logger.TryFormatException(
                            $"[StackPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
            }

            for (int i = 0; i < initialAmount; i++)
            {
                var newElement = initialAllocationCommand.AllocationDelegate(); 
                
                initialAllocationCommand.AllocationCallback?.OnAllocated(newElement);
                
                stack.Push(
                    newElement);
            }
        }
        
        #endregion

        #region StackManagedPool

        public static StackManagedPool<T> BuildStackManagedPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            ILoggerResolver loggerResolver,

            MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<StackManagedPool<T>>();
            
            var stack = new Stack<IPoolElementFacade<T>>();
            
            Func<IPoolElementFacade<T> > facadeAllocationDelegate = 
                () => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
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

            PerformInitialAllocation<T>(
                stack,
                initialFacadeAllocationCommand,
                initialAllocationCommand,
                logger);

            return new StackManagedPool<T>(
                stack,
                additionalFacadeAllocationCommand,
                additionalAllocationCommand,
                logger);
        }
        
        private static void PerformInitialAllocation<T>(
            Stack<IPoolElementFacade<T>> stack,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            ILogger logger)
        {
            int initialAmount = -1;

            switch (facadeAllocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ZERO:
                    initialAmount = 0;
                    break;

                case EAllocationAmountRule.ADD_ONE:
                    initialAmount = 1;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    initialAmount = facadeAllocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception(
                        logger.TryFormatException(
                            $"[StackPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
            }

            for (int i = 0; i < initialAmount; i++)
            {
                var newElementFacade = facadeAllocationCommand.AllocationDelegate(); 
                
                //MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
                //facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);
                
                var newElementValue = valueAllocationCommand.AllocationDelegate();
                    
                valueAllocationCommand.AllocationCallback?.OnAllocated(
                    newElementValue);

                newElementFacade.Value = newElementValue;
                
                //THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
                newElementFacade.Status = EPoolElementStatus.PUSHED;
                
                facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);
                
                stack.Push(
                    newElementFacade);
            }
        }
        
        #endregion

        #region AppendableStackManagedPool
        
        public static AppendableStackManagedPool<T> BuildAppendableStackManagedPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand,
            ILoggerResolver loggerResolver,

            MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<StackManagedPool<T>>();
            
            var stack = new Stack<IPoolElementFacade<T>>();
            
            Func<IPoolElementFacade<T> > facadeAllocationDelegate = 
                () => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
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

            PerformInitialAllocation<T>(
                stack,
                initialFacadeAllocationCommand,
                initialAllocationCommand,
                logger);

            return new AppendableStackManagedPool<T>(
                stack,
                additionalFacadeAllocationCommand,
                additionalAllocationCommand,
                
                appendFacadeAllocationCommand,
                nullValueAllocationCommand,
                
                logger);
        }
        
        #endregion
        
        #endregion

        #region Resize

        public static int ResizeStackPool<T>(
            Stack<T> stack,
            int currentCapacity,
            AllocationCommand<T> allocationCommand,
            ILogger logger)
        {
            int addedCapacity = -1;

            switch (allocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ADD_ONE:
                    addedCapacity = 1;
                    break;
                
                case EAllocationAmountRule.DOUBLE_AMOUNT:
                    addedCapacity = currentCapacity * 2;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    addedCapacity = allocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception(
                        logger.TryFormatException(
                            $"[StackPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
            }

            for (int i = 0; i < addedCapacity; i++)
            {
                var newElement = allocationCommand.AllocationDelegate(); 
                
                allocationCommand.AllocationCallback?.OnAllocated(newElement);
                
                stack.Push(
                    newElement);
            }

            return currentCapacity + addedCapacity;
        }
        
        public static int ResizeStackManagedPool<T>(
            Stack<IPoolElementFacade<T>> stack,
            int currentCapacity,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            ILogger logger)
        {
            int addedCapacity = -1;

            switch (facadeAllocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ADD_ONE:
                    addedCapacity = 1;
                    break;
                
                case EAllocationAmountRule.DOUBLE_AMOUNT:
                    addedCapacity = currentCapacity * 2;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    addedCapacity = facadeAllocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception(
                        logger.TryFormatException(
                            $"[StackPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
            }

            for (int i = 0; i < addedCapacity; i++)
            {
                var newElement = facadeAllocationCommand.AllocationDelegate(); 
                
                //MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
                //facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);
                
                var newElementValue = valueAllocationCommand.AllocationDelegate();
                    
                valueAllocationCommand.AllocationCallback?.OnAllocated(
                    newElementValue);

                newElement.Value = newElementValue;

                //THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
                newElement.Status = EPoolElementStatus.PUSHED;
                
                facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);
                
                stack.Push(
                    newElement);
            }

            return currentCapacity + addedCapacity;
        }
        
        #endregion
    }
}
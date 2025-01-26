using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public class ManagedPoolBuilder<T>
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly ILogger logger;

        
        private Func<T> valueAllocationDelegate;

        
        private Func<MetadataAllocationDescriptor>[] metadataDescriptorBuilders;

        
        private AllocationCommandDescriptor initialAllocation;

        private AllocationCommandDescriptor additionalAllocation;

        
        private IAllocationCallback<IPoolElementFacade<T>>[] facadeAllocationCallbacks;

        private IAllocationCallback<T>[] valueAllocationCallbacks;

        public ManagedPoolBuilder(
            ILoggerResolver loggerResolver,
            ILogger logger)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        public void Initialize(
            Func<T> valueAllocationDelegate,
            
            Func<MetadataAllocationDescriptor>[] metadataDescriptorBuilders,
            
            AllocationCommandDescriptor initialAllocation,
            AllocationCommandDescriptor additionalAllocation,

            IAllocationCallback<IPoolElementFacade<T>>[] facadeAllocationCallbacks = null,
            IAllocationCallback<T>[] valueAllocationCallbacks = null)
        {
            this.valueAllocationDelegate = valueAllocationDelegate;

            this.metadataDescriptorBuilders = metadataDescriptorBuilders;

            
            this.initialAllocation = initialAllocation;

            this.additionalAllocation = additionalAllocation;

            
            this.facadeAllocationCallbacks = facadeAllocationCallbacks;

            this.valueAllocationCallbacks = valueAllocationCallbacks;
        }

        #region Packed array

        public PackedArrayManagedPool<T> BuildPackedArrayManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = PackedArrayPoolFactory.BuildPackedArrayManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback,
                true);

            Cleanup();

            return result;
        }

        public ConcurrentPackedArrayManagedPool<T> BuildConcurrentPackedArrayManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = PackedArrayPoolFactory.BuildConcurrentPackedArrayManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback,
                true);

            Cleanup();

            return result;
        }

        public AppendablePackedArrayManagedPool<T> BuildAppendablePackedArrayManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = PackedArrayPoolFactory.BuildAppendableManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentAppendablePackedArrayManagedPool<T> BuildConcurrentAppendablePackedArrayManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = PackedArrayPoolFactory.BuildConcurrentAppendableManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        #endregion

        #region Stack

        public StackManagedPool<T> BuildStackManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = StackPoolFactory.BuildStackManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentStackManagedPool<T> BuildConcurrentStackManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = StackPoolFactory.BuildConcurrentStackManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public AppendableStackManagedPool<T> BuildAppendableStackManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = StackPoolFactory.BuildAppendableStackManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentAppendableStackManagedPool<T> BuildConcurrentAppendableStackManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = StackPoolFactory.BuildConcurrentAppendableStackManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        #endregion

        #region Queue

        public QueueManagedPool<T> BuildQueueManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = QueuePoolFactory.BuildQueueManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentQueueManagedPool<T> BuildConcurrentQueueManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = QueuePoolFactory.BuildConcurrentQueueManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public AppendableQueueManagedPool<T> BuildAppendableQueueManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = QueuePoolFactory.BuildAppendableQueueManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentAppendableQueueManagedPool<T> BuildConcurrentAppendableQueueManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = QueuePoolFactory.BuildConcurrentAppendableQueueManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        #endregion

        #region Linked list

        public LinkedListManagedPool<T> BuildLinkedListManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = LinkedListPoolFactory.BuildLinkedListManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentLinkedListManagedPool<T> BuildConcurrentLinkedListManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = LinkedListPoolFactory.BuildConcurrentLinkedListManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public AppendableLinkedListManagedPool<T> BuildAppendableLinkedListManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = LinkedListPoolFactory.BuildAppendableLinkedListManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        public ConcurrentAppendableLinkedListManagedPool<T> BuildConcurrentAppendableLinkedListManagedPool()
        {
            if (valueAllocationDelegate == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            var metadataDescriptors = BuildMetadataDescriptors();

            var facadeAllocationCallback = BuildFacadeAllocationCallback();

            var valueAllocationCallback = BuildValueAllocationCallback();

            var result = LinkedListPoolFactory.BuildConcurrentAppendableLinkedListManagedPool(
                new AllocationCommand<T>
                {
                    Descriptor = initialAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                new AllocationCommand<T>
                {
                    Descriptor = additionalAllocation,

                    AllocationDelegate = valueAllocationDelegate,

                    AllocationCallback = valueAllocationCallback
                },
                loggerResolver,
                metadataDescriptors,
                facadeAllocationCallback);

            Cleanup();

            return result;
        }

        #endregion

        private MetadataAllocationDescriptor[] BuildMetadataDescriptors()
        {
            List<MetadataAllocationDescriptor> metadataDescriptorsList = new List<MetadataAllocationDescriptor>();

            foreach (var descriptorBuilder in metadataDescriptorBuilders)
            {
                if (descriptorBuilder != null)
                    metadataDescriptorsList.Add(descriptorBuilder?.Invoke());
            }

            return metadataDescriptorsList.ToArray();
        }

        private IAllocationCallback<IPoolElementFacade<T>> BuildFacadeAllocationCallback()
        {
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null;
            
            if (facadeAllocationCallbacks != null)
            {
                facadeAllocationCallback = AllocationCallbackFactory.BuildCompositeCallback(
                    facadeAllocationCallbacks);
            }

            return facadeAllocationCallback;
        }
        
        private IAllocationCallback<T> BuildValueAllocationCallback()
        {
            IAllocationCallback<T> valueAllocationCallback = null;

            if (valueAllocationCallbacks != null)
            {
                valueAllocationCallback = AllocationCallbackFactory.BuildCompositeCallback(
                    valueAllocationCallbacks);
            }

            return valueAllocationCallback;
        }

        private void Cleanup()
        {
            valueAllocationDelegate = null;
            
            metadataDescriptorBuilders = null;
            
            initialAllocation = default(AllocationCommandDescriptor);
            
            additionalAllocation = default(AllocationCommandDescriptor);
            
            facadeAllocationCallbacks = null;
            
            valueAllocationCallbacks = null;
        }
    }
}
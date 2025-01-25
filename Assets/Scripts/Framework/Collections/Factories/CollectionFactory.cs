using System;
using System.Runtime.InteropServices;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Unmanaged;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Collections.Factories
{
    public static class CollectionFactory
    {
        #region Factory settings

        #region Non alloc B+ tree node pool

        public const int DEFAULT_NON_ALLOC_BPLUS_TREE_NODE_POOL_SIZE = 32;

        public static AllocationCommandDescriptor NonAllocBPlusTreeNodePoolInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_NON_ALLOC_BPLUS_TREE_NODE_POOL_SIZE
            };

        public static AllocationCommandDescriptor NonAllocBPlusTreeNodePoolAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_NON_ALLOC_BPLUS_TREE_NODE_POOL_SIZE
            };

        #endregion

        #region Non alloc B+ tree map node pool

        public const int DEFAULT_NON_ALLOC_BPLUS_TREE_MAP_NODE_POOL_SIZE = 32;

        public static AllocationCommandDescriptor NonAllocBPlusTreeMapNodePoolInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_NON_ALLOC_BPLUS_TREE_MAP_NODE_POOL_SIZE
            };

        public static AllocationCommandDescriptor NonAllocBPlusTreeMapNodePoolAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_NON_ALLOC_BPLUS_TREE_MAP_NODE_POOL_SIZE
            };

        #endregion

        public const int DEFAULT_B_PLUS_TREE_DEGREE = 32;

        public static int BPlusTreeDegree => DEFAULT_B_PLUS_TREE_DEGREE;


        public const int DEFAULT_CIRCULAR_BUFFER_CAPACITY = 1024;

        public static int CircularBufferCapacity => DEFAULT_CIRCULAR_BUFFER_CAPACITY;

        #endregion

        #region Managed

        #region B+ trees

        public static BPlusTree<T> BuildBPlusTree<T>(
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            return new BPlusTree<T>(
                degree);
        }

        public static IPool<NonAllocBPlusTreeNode<T>> BuildNonAllocBPlusTreeNodePool<T>(
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBPlusTreeNode<T>> allocationDelegate = AllocationFactory.
                ActivatorAllocationDelegate<NonAllocBPlusTreeNode<T>>;

            return StackPoolFactory.BuildStackPool<NonAllocBPlusTreeNode<T>>(
                new AllocationCommand<NonAllocBPlusTreeNode<T>>
                {
                    Descriptor = NonAllocBPlusTreeNodePoolInitialAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                new AllocationCommand<NonAllocBPlusTreeNode<T>>
                {
                    Descriptor = NonAllocBPlusTreeNodePoolAdditionalAllocationDescriptor,
                    
                    AllocationDelegate = allocationDelegate
                },
                loggerResolver);
        }

        public static NonAllocBPlusTree<T> BuildNonAllocBPlusTree<T>(
            ILoggerResolver loggerResolver,
            IPool<NonAllocBPlusTreeNode<T>> nodePool = null,
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            if (nodePool != null)
            {
                nodePool = BuildNonAllocBPlusTreeNodePool<T>(
                    loggerResolver);
            }

            return new NonAllocBPlusTree<T>(
                nodePool,
                degree);
        }

        public static ConcurrentBPlusTree<T> BuildConcurrentBPlusTree<T>(
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            return new ConcurrentBPlusTree<T>(
                degree);
        }

        public static ConcurrentNonAllocBPlusTree<T> BuildConcurrentNonAllocBPlusTree<T>(
            ILoggerResolver loggerResolver,
            IPool<NonAllocBPlusTreeNode<T>> nodePool = null,
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            if (nodePool != null)
            {
                nodePool = BuildNonAllocBPlusTreeNodePool<T>(
                    loggerResolver);
            }

            return new ConcurrentNonAllocBPlusTree<T>(
                nodePool,
                degree);
        }

        public static BPlusTreeMap<TKey, TValue> BuildBPlusTreeMap<TKey, TValue>(
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            return new BPlusTreeMap<TKey, TValue>(
                degree);
        }

        public static IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBPlusTreeMapNode<TKey, TValue>> allocationDelegate = AllocationFactory.
                ActivatorAllocationDelegate<NonAllocBPlusTreeMapNode<TKey, TValue>>;

            return StackPoolFactory.BuildStackPool<NonAllocBPlusTreeMapNode<TKey, TValue>>(
                new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>
                {
                    Descriptor = NonAllocBPlusTreeMapNodePoolInitialAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>
                {
                    Descriptor = NonAllocBPlusTreeMapNodePoolAdditionalAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                loggerResolver);
        }

        public static NonAllocBPlusTreeMap<TKey, TValue> BuildNonAllocBPlusTreeMap<TKey, TValue>(
            ILoggerResolver loggerResolver,
            IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool = null,
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            if (nodePool != null)
            {
                nodePool = BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
                    loggerResolver);
            }

            return new NonAllocBPlusTreeMap<TKey, TValue>(
                nodePool,
                degree);
        }

        public static ConcurrentBPlusTreeMap<TKey, TValue> BuildConcurrentBPlusTreeMap<TKey, TValue>(
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            return new ConcurrentBPlusTreeMap<TKey, TValue>(
                degree);
        }

        public static ConcurrentNonAllocBPlusTreeMap<TKey, TValue> BuildConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
            ILoggerResolver loggerResolver,
            IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool = null,
            int degree = DEFAULT_B_PLUS_TREE_DEGREE)
        {
            if (nodePool != null)
            {
                nodePool = BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
                    loggerResolver);
            }

            return new ConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
                nodePool,
                degree);
        }

        #endregion

        #region Circular buffers

        public static MPMCCircularBuffer<T> BuildMPMCCircularBuffer<T>()
        {
            var buffer = new T[CircularBufferCapacity];

            for (int i = 0; i < CircularBufferCapacity; i++)
            {
                buffer[i] = default;
            }

            var states = new AtomicElementState[CircularBufferCapacity];

            for (int i = 0; i < CircularBufferCapacity; i++)
            {
                states[i] = new AtomicElementState(
                    EBufferElementState.CONSUMED);
            }

            return new MPMCCircularBuffer<T>(
                buffer,
                states);
        }

        #endregion

        #endregion

        #region Unmanaged

        /// <summary>
        /// Builds an instance of UnmanagedArray.
        /// </summary>
        /// <param name="memoryPointer">A pointer to the memory location of the array.</param>
        /// <param name="elementSize">The size of each element in bytes.</param>
        /// <param name="elementCapacity">The initial capacity of the array.</param>
        /// <returns>An instance of UnmanagedArray.</returns>
        public unsafe static UnmanagedArray BuildUnmanagedArray(
            byte* memoryPointer,
            int elementSize,
            int elementCapacity = 0)
        {
            return new UnmanagedArray(
                memoryPointer,
                elementSize * elementCapacity,
                elementSize,
                elementCapacity);
        }

        /// <summary>
        /// Builds an instance of UnmanagedArray using the generic type parameter.
        /// </summary>
        /// <typeparam name="T">The type of the elements stored in the array.</typeparam>
        /// <param name="memoryPointer">A pointer to the memory location of the array.</param>
        /// <param name="elementCapacity">The initial capacity of the array.</param>
        /// <returns>An instance of UnmanagedArray.</returns>
        public unsafe static UnmanagedArray BuildUnmanagedArrayGeneric<T>(
            byte* memoryPointer,
            int elementCapacity = 0)
        {
            int elementSize = Marshal.SizeOf(typeof(T));

            return new UnmanagedArray(
                memoryPointer,
                elementSize * elementCapacity,
                elementSize,
                elementCapacity);
        }

        /// <summary>
        /// Resizes an existing UnmanagedArray.
        /// </summary>
        /// <param name="array">The UnmanagedArray to resize.</param>
        /// <param name="newMemoryPointer">A pointer to the new memory location of the array.</param>
        /// <param name="newElementCapacity">The new capacity of the array.</param>
        public unsafe static void ResizeUnmanagedArray(
            ref UnmanagedArray array,
            byte* newMemoryPointer,
            int newElementCapacity)
        {
            Buffer.MemoryCopy(
                array.MemoryPointer,
                newMemoryPointer,
                newElementCapacity * array.ElementSize,
                array.ElementCapacity * array.ElementSize);

            array.ElementCapacity = newElementCapacity;
        }

        #endregion
    }
}
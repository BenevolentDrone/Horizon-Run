using System;
using System.Threading;
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

        public static int BPlusTreeDegree = DEFAULT_B_PLUS_TREE_DEGREE;


        public const int DEFAULT_CIRCULAR_BUFFER_CAPACITY = 1024;

        public static int CircularBufferCapacity = DEFAULT_CIRCULAR_BUFFER_CAPACITY;

        #endregion

        #region Managed

        #region B+ trees

        public static BPlusTree<T> BuildBPlusTree<T>()
        {
            return new BPlusTree<T>(
                BPlusTreeDegree);
        }

        public static BPlusTree<T> BuildBPlusTree<T>(
            int degree)
        {
            return new BPlusTree<T>(
                degree);
        }

        public static IPool<NonAllocBPlusTreeNode<T>> BuildNonAllocBPlusTreeNodePool<T>(
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocBPlusTreeNodePool<T>(
                NonAllocBPlusTreeNodePoolInitialAllocationDescriptor,
                NonAllocBPlusTreeNodePoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static IPool<NonAllocBPlusTreeNode<T>> BuildNonAllocBPlusTreeNodePool<T>(
            AllocationCommandDescriptor initialAllocationDescriptor,
            AllocationCommandDescriptor additionalAllocationDescriptor,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBPlusTreeNode<T>> allocationDelegate = AllocationFactory.
                ActivatorAllocationDelegate<NonAllocBPlusTreeNode<T>>;

            return StackPoolFactory.BuildStackPool<NonAllocBPlusTreeNode<T>>(
                new AllocationCommand<NonAllocBPlusTreeNode<T>>
                {
                    Descriptor = initialAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                new AllocationCommand<NonAllocBPlusTreeNode<T>>
                {
                    Descriptor = additionalAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                loggerResolver);
        }

        public static NonAllocBPlusTree<T> BuildNonAllocBPlusTree<T>(
            ILoggerResolver loggerResolver)
        {
            var nodePool = BuildNonAllocBPlusTreeNodePool<T>(
                loggerResolver);

            return new NonAllocBPlusTree<T>(
                nodePool,
                BPlusTreeDegree);
        }

        public static NonAllocBPlusTree<T> BuildNonAllocBPlusTree<T>(
            IPool<NonAllocBPlusTreeNode<T>> nodePool,
            int degree)
        {
            return new NonAllocBPlusTree<T>(
                nodePool,
                degree);
        }

        public static ConcurrentBPlusTree<T> BuildConcurrentBPlusTree<T>()
        {
            return new ConcurrentBPlusTree<T>(
                BuildBPlusTree<T>(BPlusTreeDegree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentBPlusTree<T> BuildConcurrentBPlusTree<T>(
            int degree)
        {
            return new ConcurrentBPlusTree<T>(
                BuildBPlusTree<T>(degree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentNonAllocBPlusTree<T> BuildConcurrentNonAllocBPlusTree<T>(
            ILoggerResolver loggerResolver)
        {
            var nodePool = BuildNonAllocBPlusTreeNodePool<T>(
                loggerResolver);

            return new ConcurrentNonAllocBPlusTree<T>(
                BuildNonAllocBPlusTree<T>(
                    nodePool,
                    BPlusTreeDegree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentNonAllocBPlusTree<T> BuildConcurrentNonAllocBPlusTree<T>(
            IPool<NonAllocBPlusTreeNode<T>> nodePool,
            int degree)
        {
            return new ConcurrentNonAllocBPlusTree<T>(
                BuildNonAllocBPlusTree<T>(
                    nodePool,
                    degree),
                new SemaphoreSlim(1, 1));
        }

        public static BPlusTreeMap<TKey, TValue> BuildBPlusTreeMap<TKey, TValue>()
        {
            return new BPlusTreeMap<TKey, TValue>(
                BPlusTreeDegree);
        }

        public static BPlusTreeMap<TKey, TValue> BuildBPlusTreeMap<TKey, TValue>(
            int degree)
        {
            return new BPlusTreeMap<TKey, TValue>(
                degree);
        }

        public static IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
                NonAllocBPlusTreeMapNodePoolInitialAllocationDescriptor,
                NonAllocBPlusTreeMapNodePoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
            AllocationCommandDescriptor initialAllocationDescriptor,
            AllocationCommandDescriptor additionalAllocationDescriptor,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBPlusTreeMapNode<TKey, TValue>> allocationDelegate = AllocationFactory.
                ActivatorAllocationDelegate<NonAllocBPlusTreeMapNode<TKey, TValue>>;

            return StackPoolFactory.BuildStackPool<NonAllocBPlusTreeMapNode<TKey, TValue>>(
                new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>
                {
                    Descriptor = initialAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>
                {
                    Descriptor = additionalAllocationDescriptor,

                    AllocationDelegate = allocationDelegate
                },
                loggerResolver);
        }

        public static NonAllocBPlusTreeMap<TKey, TValue> BuildNonAllocBPlusTreeMap<TKey, TValue>(
            ILoggerResolver loggerResolver)
        {
            var nodePool = BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
                    loggerResolver);

            return new NonAllocBPlusTreeMap<TKey, TValue>(
                nodePool,
                BPlusTreeDegree);
        }

        public static NonAllocBPlusTreeMap<TKey, TValue> BuildNonAllocBPlusTreeMap<TKey, TValue>(
            IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool,
            int degree)
        {
            return new NonAllocBPlusTreeMap<TKey, TValue>(
                nodePool,
                degree);
        }

        public static ConcurrentBPlusTreeMap<TKey, TValue> BuildConcurrentBPlusTreeMap<TKey, TValue>()
        {
            return new ConcurrentBPlusTreeMap<TKey, TValue>(
                BuildBPlusTreeMap<TKey, TValue>(
                    BPlusTreeDegree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentBPlusTreeMap<TKey, TValue> BuildConcurrentBPlusTreeMap<TKey, TValue>(
            int degree)
        {
            return new ConcurrentBPlusTreeMap<TKey, TValue>(
                BuildBPlusTreeMap<TKey, TValue>(
                    degree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentNonAllocBPlusTreeMap<TKey, TValue> BuildConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
            ILoggerResolver loggerResolver)
        {
            var nodePool = BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
                    loggerResolver);

            return new ConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
                BuildNonAllocBPlusTreeMap<TKey, TValue>(
                    nodePool,
                    BPlusTreeDegree),
                new SemaphoreSlim(1, 1));
        }

        public static ConcurrentNonAllocBPlusTreeMap<TKey, TValue> BuildConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
            IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool,
            int degree)
        {
            return new ConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
                BuildNonAllocBPlusTreeMap<TKey, TValue>(
                    nodePool,
                    degree),
                new SemaphoreSlim(1, 1));
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
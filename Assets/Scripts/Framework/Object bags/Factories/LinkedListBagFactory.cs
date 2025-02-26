using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Bags.Factories
{
	public static class LinkedListBagFactory
	{
		#region Factory settings

		#region Linked list node pool

		public const int DEFAULT_LINKED_LIST_NODE_POOL_SIZE = 32;

		public static AllocationCommandDescriptor LinkedListNodePoolInitialAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_LINKED_LIST_NODE_POOL_SIZE
			};

		public static AllocationCommandDescriptor LinkedListNodePoolAdditionalAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_LINKED_LIST_NODE_POOL_SIZE
			};

		#endregion

		#endregion

		#region Linked list bag

		public static LinkedListBag<T> BuildLinkedListBag<T>()
		{
			var linkedList = new LinkedList<T>();

			return new LinkedListBag<T>(
				linkedList);
		}

		#endregion

		#region Non alloc linked list bag

		public static NonAllocLinkedListBag<T> BuildNonAllocLinkedListBag<T>(
			ILoggerResolver loggerResolver)
		{
			return BuildNonAllocLinkedListBag<T>(
				LinkedListNodePoolInitialAllocationDescriptor,
				LinkedListNodePoolAdditionalAllocationDescriptor,
				loggerResolver);
		}

		public static NonAllocLinkedListBag<T> BuildNonAllocLinkedListBag<T>(
			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor,
			ILoggerResolver loggerResolver)
		{
			var linkedList = new LinkedList<T>();

			Func<LinkedListNode<T>> allocationDelegate = AllocationFactory
				.ActivatorAllocationDelegate<LinkedListNode<T>>;

			return new NonAllocLinkedListBag<T>(
				linkedList,
				StackPoolFactory.BuildStackPool<LinkedListNode<T>>(
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = initialAllocationDescriptor,

						AllocationDelegate = allocationDelegate
					},
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = additionalAllocationDescriptor,

						AllocationDelegate = allocationDelegate
					},
					loggerResolver));
		}

		#endregion

		#region Concurrent linked list bag

		public static ConcurrentLinkedListBag<T> BuildConcurrentLinkedListBag<T>()
		{
			return new ConcurrentLinkedListBag<T>(
				BuildLinkedListBag<T>(),
				new SemaphoreSlim(1, 1));
		}

		#endregion

		#region Concurrent non alloc linked list bag

		public static ConcurrentNonAllocLinkedListBag<T> BuildConcurrentNonAllocLinkedListBag<T>(
			ILoggerResolver loggerResolver)
		{
			return BuildConcurrentNonAllocLinkedListBag<T>(
				LinkedListNodePoolInitialAllocationDescriptor,
				LinkedListNodePoolAdditionalAllocationDescriptor,
				loggerResolver);
		}

		public static ConcurrentNonAllocLinkedListBag<T> BuildConcurrentNonAllocLinkedListBag<T>(
			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor,
			ILoggerResolver loggerResolver)
		{
			return new ConcurrentNonAllocLinkedListBag<T>(
				BuildNonAllocLinkedListBag<T>(
					initialAllocationDescriptor,
					additionalAllocationDescriptor,
					loggerResolver),
				new SemaphoreSlim(1, 1));
		}

		#endregion
	}
}
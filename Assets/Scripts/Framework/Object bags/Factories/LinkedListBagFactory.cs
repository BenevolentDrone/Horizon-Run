using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Pools.Factories;

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
			ILoggerResolver loggerResolver = null)
		{
			var linkedList = new LinkedList<T>();

			Func<LinkedListNode<T>> allocationDelegate = AllocationFactory
				.ActivatorAllocationDelegate<LinkedListNode<T>>;

			return new NonAllocLinkedListBag<T>(
				linkedList,
				StackPoolFactory.BuildStackPool<LinkedListNode<T>>(
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = LinkedListNodePoolInitialAllocationDescriptor,
						
						AllocationDelegate = allocationDelegate
					},
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = LinkedListNodePoolAdditionalAllocationDescriptor,

						AllocationDelegate = allocationDelegate
					},
					loggerResolver));
		}

		#endregion
	}
}
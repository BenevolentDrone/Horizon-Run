using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Pools.Factories;

namespace HereticalSolutions.Bags.Factories
{
	public static class LinkedListBagFactory
	{
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

			return new NonAllocLinkedListBag<T>(
				linkedList,
				StackPoolFactory.BuildStackPool<LinkedListNode<T>>(
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = new AllocationCommandDescriptor
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

							Amount = 16 //TODO: REMOVE MAGIC
						},
						AllocationDelegate =
							AllocationFactory
								.ActivatorAllocationDelegate<LinkedListNode<T>>
					},
					new AllocationCommand<LinkedListNode<T>>
					{
						Descriptor = new AllocationCommandDescriptor
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

							Amount = 16 //TODO: REMOVE MAGIC
						},
						AllocationDelegate =
							AllocationFactory
								.ActivatorAllocationDelegate<LinkedListNode<T>>
					},
					loggerResolver));
		}

		#endregion
	}
}
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.SpacePartitioning.Factories
{
	public static class NodePoolFactory
	{
		private const int INITIAL_NODE_POOL_SIZE = 128;

		public static IPool<Node<TValue>> BuildNodePool<TValue>(
			ILoggerResolver loggerResolver = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<Node<TValue>>()
				?? null;

			return PoolsFactory.BuildStackPool<Node<TValue>>(
				new AllocationCommand<Node<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
						Amount = 5
					},
					AllocationDelegate = () => new Node<TValue>(
						new Node<TValue>[4],
						new List<ValueSpaceData<TValue>>(),
						-1,
						logger),
				},
				new AllocationCommand<Node<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = INITIAL_NODE_POOL_SIZE
					},
					AllocationDelegate = () => new Node<TValue>(
						new Node<TValue>[4],
						new List<ValueSpaceData<TValue>>(),
						-1,
						logger),
				});
		}

		public static IPool<ValueSpaceData<TValue>> BuildValueDataPool<TValue>()
		{
			return PoolsFactory.BuildStackPool<ValueSpaceData<TValue>>(
				new AllocationCommand<ValueSpaceData<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = 5
					},
					AllocationDelegate = () => new ValueSpaceData<TValue>(),
				},
				new AllocationCommand<ValueSpaceData<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = INITIAL_NODE_POOL_SIZE
					},
					AllocationDelegate = () => new ValueSpaceData<TValue>(),
				});
		}
	}
}
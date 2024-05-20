using System;
using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public class SpawnPooledGameObjectViewSystem : IDefaultECSEntityInitializationSystem
	{
		private readonly INonAllocDecoratedPool<GameObject> pool;


		private readonly AddressArgument addressArgument;

		private readonly IPoolDecoratorArgument[] arguments;


		private readonly ILogger logger;

		public SpawnPooledGameObjectViewSystem(
			INonAllocDecoratedPool<GameObject> pool,
			ILogger logger = null)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();

			arguments = new IPoolDecoratorArgument[]
			{
				addressArgument
			};
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<SpawnPooledGameObjectViewComponent>())
				return;


			ref SpawnPooledGameObjectViewComponent spawnViewComponent = ref entity.Get<SpawnPooledGameObjectViewComponent>();

			string address = spawnViewComponent.Address;


			addressArgument.FullAddress = address;
			addressArgument.AddressHashes = address.AddressToHashes();

			var pooledViewElement = pool
				.Pop(arguments);

			if (pooledViewElement.Value == null)
			{
				throw new Exception(
					logger.TryFormat<SpawnPooledGameObjectViewSystem>(
						$"POOLED ELEMENT'S VALUE IS NULL"));
			}

			if (pooledViewElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormat<SpawnPooledGameObjectViewSystem>(
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledViewElement.Value.name})"));
			}
			
			if (!pooledViewElement.Value.activeInHierarchy)
			{
				throw new Exception(
					logger.TryFormat<SpawnPooledGameObjectViewSystem>(
						$"POOLED GAME OBJECT IS SPAWNED DISABLED ({pooledViewElement.Value.name})"));
			}


			var pooledGameObjectViewComponent = new PooledGameObjectViewComponent();

			pooledGameObjectViewComponent.Element = pooledViewElement;

			entity.Set<PooledGameObjectViewComponent>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectViewComponent>();


			var viewEntityAdapter = pooledViewElement.Value.GetComponentInChildren<GameObjectViewEntityAdapter>();

			if (viewEntityAdapter == null)
			{
				logger?.LogError<SpawnPooledGameObjectViewSystem>(
					$"NO VIEW ENTITY ADAPTER ON POOLED GAME OBJECT",
					new object[]
					{
						pooledViewElement.Value
					});
				
				UnityEngine.Debug.Break();
				
				return;
			}
			
			if (viewEntityAdapter.ViewEntity.IsAlive)
			{
				logger?.LogError<SpawnPooledGameObjectViewSystem>(
					$"VIEW ENTITY ADAPTER'S ENTITY IS STILL ALIVE (CURRENT ENTITY: {viewEntityAdapter.ViewEntity} DESIRED ENTITY: {entity})",
					new object[]
					{
						pooledViewElement.Value
					});
				
				UnityEngine.Debug.Break();
				
				return;
			}

			if (viewEntityAdapter != null)
			{
				viewEntityAdapter.Initialize(entity);
			}
		}

		public void Dispose()
		{
		}
	}
}
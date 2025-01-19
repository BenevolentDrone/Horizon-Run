using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class SpawnPooledGameObjectViewSystem
		: IEntityInitializationSystem
	{
		private readonly IManagedPool<GameObject> pool;


		private readonly AddressArgument addressArgument;

		private readonly IPoolPopArgument[] arguments;


		private readonly ILogger logger;

		public SpawnPooledGameObjectViewSystem(
			IManagedPool<GameObject> pool,
			ILogger logger)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();

			arguments = new IPoolPopArgument[]
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
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S VALUE IS NULL ({address})"));
			}

			if (pooledViewElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledViewElement.Value.name})"));
			}
			
			if (!pooledViewElement.Value.activeInHierarchy)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED GAME OBJECT IS SPAWNED DISABLED ({pooledViewElement.Value.name})"));
			}


			var pooledGameObjectViewComponent = new PooledGameObjectViewComponent();

			pooledGameObjectViewComponent.ElementFacade = pooledViewElement;

			entity.Set<PooledGameObjectViewComponent>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectViewComponent>();


			var viewEntityAdapter = pooledViewElement.Value.GetComponentInChildren<GameObjectViewEntityAdapter>();

			if (viewEntityAdapter == null)
			{
				logger?.LogError(
					GetType(),
					$"NO VIEW ENTITY ADAPTER ON POOLED GAME OBJECT",
					new object[]
					{
						pooledViewElement.Value
					});
				
				return;
			}
			
			if (viewEntityAdapter.ViewEntity.IsAlive)
			{
				logger?.LogError(
					GetType(),
					$"VIEW ENTITY ADAPTER'S ENTITY IS STILL ALIVE (CURRENT ENTITY: {viewEntityAdapter.ViewEntity} DESIRED ENTITY: {entity})",
					new object[]
					{
						pooledViewElement.Value
					});
				
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
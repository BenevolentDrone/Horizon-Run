using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class ResolvePooledGameObjectViewSystem<TSceneEntity>
		: IEntityInitializationSystem
		  where TSceneEntity : MonoBehaviour
	{
		private readonly IManagedPool<GameObject> pool;

		private readonly AddressArgument addressArgument;
		private readonly AppendToPoolArgument appendToPoolArgument;
		private readonly IPoolPopArgument[] arguments;

		private readonly ILogger logger;

		public ResolvePooledGameObjectViewSystem(
			IManagedPool<GameObject> pool,
			ILogger logger = null)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();
			appendToPoolArgument = new AppendToPoolArgument();

			arguments = new IPoolPopArgument[]
			{
				addressArgument,
				appendToPoolArgument
			};
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<ResolveComponent>())
				return;

			if (!entity.Has<SpawnPooledGameObjectViewComponent>())
			{
				return;

				//throw new Exception(
				//	logger.TryFormatException(
				//		GetType(),
				//		$"ENTITY {entity.Get<GUIDComponent>().GUID} WAS REQUESTED TO BE RESOLVED BUT HAS NO SpawnPooledGameObjectViewComponent"));
			}

			ref ResolveComponent ResolveComponent = ref entity.Get<ResolveComponent>();

			ref SpawnPooledGameObjectViewComponent spawnViewComponent = ref entity.Get<SpawnPooledGameObjectViewComponent>();

			string address = spawnViewComponent.Address;


			addressArgument.FullAddress = address;
			addressArgument.AddressHashes = address.AddressToHashes();

			var pooledViewElement = pool
				.Pop(arguments);

			if (pooledViewElement.Value != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S VALUE IS NOT NULL"));
			}

			if (pooledViewElement.Status != EPoolElementStatus.UNINITIALIZED)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledViewElement.Value.name}), EXPECTED: {EPoolElementStatus.UNINITIALIZED}"));
			}

			pooledViewElement.Value = (GameObject)ResolveComponent.Source;

			pooledViewElement.Status = EPoolElementStatus.POPPED;


			var pooledGameObjectViewComponent = new PooledGameObjectViewComponent();

			pooledGameObjectViewComponent.ElementFacade = pooledViewElement;

			entity.Set<PooledGameObjectViewComponent>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectViewComponent>();


			var sceneEntity = pooledViewElement.Value.GetComponentInChildren<TSceneEntity>();

			if (sceneEntity != null)
				GameObject.Destroy(sceneEntity);


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

		/// <summary>
		/// Disposes the system.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
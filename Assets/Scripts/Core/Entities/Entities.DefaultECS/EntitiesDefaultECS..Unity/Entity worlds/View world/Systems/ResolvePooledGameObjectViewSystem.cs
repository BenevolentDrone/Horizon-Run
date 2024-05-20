using System;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public class ResolvePooledGameObjectViewSystem<TSceneEntity>
		: IDefaultECSEntityInitializationSystem
		  where TSceneEntity : MonoBehaviour
	{
		private readonly INonAllocDecoratedPool<GameObject> pool;

		private readonly AddressArgument addressArgument;
		private readonly AppendArgument appendArgument;
		private readonly IPoolDecoratorArgument[] arguments;

		private readonly ILogger logger;

		public ResolvePooledGameObjectViewSystem(
			INonAllocDecoratedPool<GameObject> pool,
			ILogger logger = null)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();
			appendArgument = new AppendArgument();

			arguments = new IPoolDecoratorArgument[]
			{
				addressArgument,
				appendArgument
			};
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<ResolveViewComponent>())
				return;

			if (!entity.Has<SpawnPooledGameObjectViewComponent>())
			{
				return;

				//throw new Exception(
				//	logger.TryFormat<ResolvePooledGameObjectViewSystem>(
				//		$"ENTITY {entity.Get<GUIDComponent>().GUID} WAS REQUESTED TO BE RESOLVED BUT HAS NO SpawnPooledGameObjectViewComponent"));
			}

			ref ResolveViewComponent resolveViewComponent = ref entity.Get<ResolveViewComponent>();

			ref SpawnPooledGameObjectViewComponent spawnViewComponent = ref entity.Get<SpawnPooledGameObjectViewComponent>();

			string address = spawnViewComponent.Address;


			addressArgument.FullAddress = address;
			addressArgument.AddressHashes = address.AddressToHashes();

			var pooledViewElement = pool
				.Pop(arguments);

			if (pooledViewElement.Value != null)
			{
				throw new Exception(
					logger.TryFormat<ResolvePooledGameObjectViewSystem<TSceneEntity>>(
						$"POOLED ELEMENT'S VALUE IS NOT NULL"));
			}

			if (pooledViewElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormat<ResolvePooledGameObjectViewSystem<TSceneEntity>>(
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledViewElement.Value.name})"));
			}

			pooledViewElement.Value = (GameObject)resolveViewComponent.Source;


			var pooledGameObjectViewComponent = new PooledGameObjectViewComponent();

			pooledGameObjectViewComponent.Element = pooledViewElement;

			entity.Set<PooledGameObjectViewComponent>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectViewComponent>();


			var sceneEntity = pooledViewElement.Value.GetComponentInChildren<TSceneEntity>();

			if (sceneEntity != null)
				GameObject.Destroy(sceneEntity);


			var viewEntityAdapter = pooledViewElement.Value.GetComponentInChildren<GameObjectViewEntityAdapter>();
			
			if (viewEntityAdapter == null)
			{
				logger?.LogError<ResolvePooledGameObjectViewSystem<TSceneEntity>>(
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
				logger?.LogError<ResolvePooledGameObjectViewSystem<TSceneEntity>>(
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

		/// <summary>
		/// Disposes the system.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
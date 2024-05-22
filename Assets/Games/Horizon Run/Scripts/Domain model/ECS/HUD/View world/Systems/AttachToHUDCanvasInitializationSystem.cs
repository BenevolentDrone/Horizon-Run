using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class AttachToHUDCanvasInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly Transform hudCanvasTransform;

		private readonly ILogger logger;

		public AttachToHUDCanvasInitializationSystem(
			Transform hudCanvasTransform,
			ILogger logger = null)
		{
			this.hudCanvasTransform = hudCanvasTransform;

			this.logger = logger;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<AttachToHUDCanvasInitializationSystem>())
				return;

			if (!entity.Has<PooledGameObjectViewComponent>())
				return;


			var pooledGameObjectViewComponent = entity.Get<PooledGameObjectViewComponent>();

			var pooledViewElement = pooledGameObjectViewComponent.Element;

			if (pooledViewElement.Value == null)
			{
				throw new Exception(
					logger.TryFormat<AttachToHUDCanvasInitializationSystem>(
						$"POOLED ELEMENT'S VALUE IS NULL"));
			}

			if (pooledViewElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormat<AttachToHUDCanvasInitializationSystem>(
						$"POOLED ELEMENT'S STATUS IS INVALID"));
			}

			if (!pooledViewElement.Value.activeInHierarchy)
			{
				throw new Exception(
					logger.TryFormat<AttachToHUDCanvasInitializationSystem>(
						$"POOLED GAME OBJECT IS DISABLED"));
			}

			var viewEntityAdapter = pooledViewElement.Value.GetComponentInChildren<GameObjectViewEntityAdapter>();

			if (viewEntityAdapter == null)
			{
				logger?.LogError<AttachToHUDCanvasInitializationSystem>(
					$"NO VIEW ENTITY ADAPTER ON POOLED GAME OBJECT",
					new object[]
					{
						pooledViewElement.Value
					});

				UnityEngine.Debug.Break();

				return;
			}

			pooledViewElement.Value.transform.SetParent(
				hudCanvasTransform,
				false);

			entity.Remove<AttachToHUDCanvasInitializationSystem>();
		}

		public void Dispose()
		{
		}
	}
}

using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class PushPooledGameObjectViewSystem : IDefaultECSEntityInitializationSystem
    {
        private readonly ILogger logger;

        public PushPooledGameObjectViewSystem(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<PooledGameObjectViewComponent>())
                return;
            
            
            var pooledGameObjectViewComponent = entity.Get<PooledGameObjectViewComponent>();

            var pooledViewElement = pooledGameObjectViewComponent.Element;
            
            if (pooledViewElement.Value == null)
            {
                throw new Exception(
                    logger.TryFormat<PushPooledGameObjectViewSystem>(
                        $"POOLED ELEMENT'S VALUE IS NULL"));
            }

            if (pooledViewElement.Status != EPoolElementStatus.POPPED)
            {
                throw new Exception(
                    logger.TryFormat<PushPooledGameObjectViewSystem>(
                        $"POOLED ELEMENT'S STATUS IS INVALID"));
            }
			
            if (!pooledViewElement.Value.activeInHierarchy)
            {
                throw new Exception(
                    logger.TryFormat<PushPooledGameObjectViewSystem>(
                        $"POOLED GAME OBJECT IS DISABLED"));
            }
            
            var viewEntityAdapter = pooledViewElement.Value.GetComponentInChildren<GameObjectViewEntityAdapter>();

            if (viewEntityAdapter == null)
            {
                logger?.LogError<PushPooledGameObjectViewSystem>(
                    $"NO VIEW ENTITY ADAPTER ON POOLED GAME OBJECT",
                    new object[]
                    {
                        pooledViewElement.Value
                    });
				
                UnityEngine.Debug.Break();
				
                return;
            }

            if (viewEntityAdapter != null)
            {
                viewEntityAdapter.Deinitialize();
            }
            
            //logger?.Log<PushPooledGameObjectViewSystem>(
            //    $"PUSHING POOLED GAME OBJECT {pooledGameObjectViewComponent.Element.Value.name} ENTITY {entity}");
            
            pooledGameObjectViewComponent.Element.Push();
            
            
            pooledGameObjectViewComponent.Element = null;
            
            entity.Remove<PooledGameObjectViewComponent>();
        }

        public void Dispose()
        {
        }
    }
}
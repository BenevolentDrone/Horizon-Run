using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class PushPooledGameObjectViewSystem : IEntityInitializationSystem
    {
        private readonly ILogger logger;

        public PushPooledGameObjectViewSystem(
            ILogger logger)
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

            var pooledViewElement = pooledGameObjectViewComponent.ElementFacade;
            
            if (pooledViewElement.Value == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"POOLED ELEMENT'S VALUE IS NULL"));
            }

            if (pooledViewElement.Status != EPoolElementStatus.POPPED)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"POOLED ELEMENT'S STATUS IS INVALID"));
            }
			
            if (!pooledViewElement.Value.activeInHierarchy)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"POOLED GAME OBJECT IS DISABLED"));
            }
            
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

            if (viewEntityAdapter != null)
            {
                viewEntityAdapter.Deinitialize();
            }
            
            //logger?.Log(
            //    GetType(),
            //    $"PUSHING POOLED GAME OBJECT {pooledGameObjectViewComponent.Element.Value.name} ENTITY {entity}");
            
            pooledGameObjectViewComponent.ElementFacade.Push();
            
            
            pooledGameObjectViewComponent.ElementFacade = null;
            
            entity.Remove<PooledGameObjectViewComponent>();
        }

        public void Dispose()
        {
        }
    }
}
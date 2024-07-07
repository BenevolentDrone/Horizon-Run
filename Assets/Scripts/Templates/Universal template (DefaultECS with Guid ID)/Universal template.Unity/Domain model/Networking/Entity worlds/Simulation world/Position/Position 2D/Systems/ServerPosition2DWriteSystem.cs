using HereticalSolutions.Networking;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class ServerPosition2DWriteSystem : ISystem<float>
    {
        private readonly NetworkPositionInterpolationSettings networkPositionInterpolationSettings;
        
        private readonly EntitySet serverPositionEntitiesSet;
        
        public bool IsEnabled { get; set; } = true;
        
        public ServerPosition2DWriteSystem(
            World world,
            NetworkPositionInterpolationSettings networkPositionInterpolationSettings,
            ILogger logger = null)
        {
            this.networkPositionInterpolationSettings = networkPositionInterpolationSettings;
            
            serverPositionEntitiesSet = world
                .GetEntities()
                .With<Position2DComponent>()
                .With<ServerPosition2DComponent>()
                .AsSet();
        }

        public void Update(
            float deltaTime)
        {
            foreach (Entity entity in serverPositionEntitiesSet.GetEntities())
            {
                ref var position2DComponent = ref entity.Get<Position2DComponent>();

                ref var serverPosition2DComponent = ref entity.Get<ServerPosition2DComponent>();

                if (serverPosition2DComponent.Dirty)
                {
                    serverPosition2DComponent.Error = serverPosition2DComponent.ServerPosition - position2DComponent.Position;

                    //position2DComponent.Position = serverPosition2DComponent.ServerPosition;

                    serverPosition2DComponent.Dirty = false;
                }
                
                var error = serverPosition2DComponent.Error;

                var errorMagnitude = error.magnitude;
                
                if (errorMagnitude < MathHelpers.EPSILON)
                    continue;
                
                if (errorMagnitude < networkPositionInterpolationSettings.PositionInterpolationThreshold)
                {
                    position2DComponent.Position += error;
                    
                    serverPosition2DComponent.Error = Vector2.zero;
                    
                    continue;
                }
                
                if (errorMagnitude > networkPositionInterpolationSettings.PositionMaxAllowedDeviation)
                {
                    var extraDeviation = error.normalized
                        * (errorMagnitude - networkPositionInterpolationSettings.PositionMaxAllowedDeviation);
                
                    position2DComponent.Position += extraDeviation;
                
                    serverPosition2DComponent.Error -= extraDeviation;
                    
                    error -= extraDeviation;
                }
                
                var errorInterpolated = Vector2.Lerp(
                    Vector2.zero,
                    error,
                    networkPositionInterpolationSettings.PositionInterpolationValue * deltaTime);
                
                position2DComponent.Position += errorInterpolated;
                
                serverPosition2DComponent.Error -= errorInterpolated;
            }
        }
        
        public void Dispose()
        {
        }
    }
}
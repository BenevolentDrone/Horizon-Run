using HereticalSolutions.Networking;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPosition3DWriteSystem : ISystem<float>
    {
        private readonly NetworkPositionInterpolationSettings networkPositionInterpolationSettings;
        
        private readonly EntitySet serverPositionEntitiesSet;
        
        public bool IsEnabled { get; set; } = true;
        
        public ServerPosition3DWriteSystem(
            World world,
            NetworkPositionInterpolationSettings networkPositionInterpolationSettings,
            ILogger logger = null)
        {
            this.networkPositionInterpolationSettings = networkPositionInterpolationSettings;
            
            serverPositionEntitiesSet = world
                .GetEntities()
                .With<Position3DComponent>()
                .With<ServerPosition3DComponent>()
                .AsSet();
        }

        public void Update(
            float deltaTime)
        {
            foreach (Entity entity in serverPositionEntitiesSet.GetEntities())
            {
                ref var position3DComponent = ref entity.Get<Position3DComponent>();

                ref var serverPosition3DComponent = ref entity.Get<ServerPosition3DComponent>();

                if (serverPosition3DComponent.Dirty)
                {
                    serverPosition3DComponent.Error = serverPosition3DComponent.ServerPosition - position3DComponent.Position;

                    //position3DComponent.Position = serverPosition3DComponent.ServerPosition;

                    serverPosition3DComponent.Dirty = false;
                }
                
                var error = serverPosition3DComponent.Error;

                var errorMagnitude = error.magnitude;
                
                if (errorMagnitude < MathHelpers.EPSILON)
                    continue;
                
                if (errorMagnitude < networkPositionInterpolationSettings.PositionInterpolationThreshold)
                {
                    position3DComponent.Position += error;
                    
                    serverPosition3DComponent.Error = Vector3.zero;
                    
                    continue;
                }
                
                if (errorMagnitude > networkPositionInterpolationSettings.PositionMaxAllowedDeviation)
                {
                    var extraDeviation = error.normalized
                        * (errorMagnitude - networkPositionInterpolationSettings.PositionMaxAllowedDeviation);
                
                    position3DComponent.Position += extraDeviation;
                
                    serverPosition3DComponent.Error -= extraDeviation;
                    
                    error -= extraDeviation;
                }
                
                var errorInterpolated = Vector3.Lerp(
                    Vector3.zero,
                    error,
                    networkPositionInterpolationSettings.PositionInterpolationValue * deltaTime);
                
                position3DComponent.Position += errorInterpolated;
                
                serverPosition3DComponent.Error -= errorInterpolated;
            }
        }
        
        public void Dispose()
        {
        }
    }
}
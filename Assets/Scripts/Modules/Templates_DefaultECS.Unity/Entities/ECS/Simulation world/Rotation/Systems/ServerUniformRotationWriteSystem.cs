using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Networking;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerUniformRotationWriteSystem : ISystem<float>
    {
        private readonly NetworkRotationInterpolationSettings networkRotationInterpolationSettings;

        private readonly EntitySet serverRotationEntitiesSet;
        
        public bool IsEnabled { get; set; } = true;
        
        public ServerUniformRotationWriteSystem(
            World world,
            NetworkRotationInterpolationSettings networkRotationInterpolationSettings,
            ILogger logger)
        {
            this.networkRotationInterpolationSettings = networkRotationInterpolationSettings;
            
            serverRotationEntitiesSet = world
                .GetEntities()
                .With<UniformRotationComponent>()
                .With<ServerUniformRotationComponent>()
                .AsSet();
        }

        public void Update(
            float deltaTime)
        {
            foreach (Entity entity in serverRotationEntitiesSet.GetEntities())
            {
                ref var rotationComponent = ref entity.Get<UniformRotationComponent>();

                ref var serverRotationComponent = ref entity.Get<ServerUniformRotationComponent>();

                if (serverRotationComponent.Dirty)
                {
                    //rotationComponent.Angle = serverRotationComponent.ServerRotation.SanitizeAngle();
                    
                    serverRotationComponent.Error = (serverRotationComponent.ServerRotation - rotationComponent.Angle).SanitizeAngle();
                    
                    if (serverRotationComponent.Error > 180f)
                        serverRotationComponent.Error -= 360f;
                    
                    if (serverRotationComponent.Error < -180f)
                        serverRotationComponent.Error += 360f;
                    
                    serverRotationComponent.Dirty = false;
                }
                
                var error = serverRotationComponent.Error;
                
                var errorMagnitude = Mathf.Abs(error);
                
                if (errorMagnitude < MathHelpers.EPSILON)
                    continue;
                
                if (errorMagnitude < networkRotationInterpolationSettings.RotationInterpolationThreshold)
                {
                    rotationComponent.Angle += error;
                    
                    rotationComponent.Angle = rotationComponent.Angle.SanitizeAngle();
                    
                    serverRotationComponent.Error = 0f;
                    
                    continue;
                }
                
                if (errorMagnitude > networkRotationInterpolationSettings.RotationMaxAllowedDeviation)
                {
                    var extraDeviation = Mathf.Sign(error) * (errorMagnitude - networkRotationInterpolationSettings.RotationMaxAllowedDeviation);
                
                    rotationComponent.Angle += extraDeviation;
                    
                    rotationComponent.Angle = rotationComponent.Angle.SanitizeAngle();
                
                    serverRotationComponent.Error -= extraDeviation;
                
                    serverRotationComponent.Error = serverRotationComponent.Error.SanitizeAngle();
                    
                    error -= extraDeviation;
                }
                
                var errorInterpolated = Mathf.Lerp(
                    0f,
                    error,
                    networkRotationInterpolationSettings.RotationInterpolationValue * deltaTime);
                
                rotationComponent.Angle += errorInterpolated;
                
                rotationComponent.Angle = rotationComponent.Angle.SanitizeAngle();
                
                serverRotationComponent.Error -= errorInterpolated;
                
                serverRotationComponent.Error = serverRotationComponent.Error.SanitizeAngle();
            }
        }
        
        public void Dispose()
        {
        }
    }
}
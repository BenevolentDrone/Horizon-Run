using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPositionRotation2DPresenterSystem : AEntitySetSystem<float>
    {
        public ServerPositionRotation2DPresenterSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<ServerPositionRotation2DPresenterComponent>()
                    .With<ServerPositionRotation2DViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var serverPositionRotation2DPresenterComponent = entity.Get<ServerPositionRotation2DPresenterComponent>();

            ref var serverPositionRotation2DViewComponent = ref entity.Get<ServerPositionRotation2DViewComponent>();


            var targetEntity = serverPositionRotation2DPresenterComponent.TargetEntity;

            if (!targetEntity.IsAlive)
            {
                serverPositionRotation2DViewComponent.Visible = false;
                
                return;
            }
            
            serverPositionRotation2DViewComponent.Visible = true;


            var serverPosition2DComponent = targetEntity.Get<ServerPosition2DComponent>();
            
            var serverUniformRotationComponent = targetEntity.Get<ServerUniformRotationComponent>();
            
            var position2DComponent = targetEntity.Get<Position2DComponent>();
            
            var uniformRotationComponent = targetEntity.Get<UniformRotationComponent>();

            
            var lastServerPosition = serverPositionRotation2DViewComponent.ServerPosition;
            
            serverPositionRotation2DViewComponent.ServerPosition = serverPosition2DComponent.ServerPosition;

            if ((lastServerPosition - serverPositionRotation2DViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
                serverPositionRotation2DViewComponent.Dirty = true;
            
            
            var lastServerRotation = serverPositionRotation2DViewComponent.ServerRotation;
            
            serverPositionRotation2DViewComponent.ServerRotation = serverUniformRotationComponent.ServerRotation;

            if (Mathf.Abs(
                    Mathf.DeltaAngle(
                        lastServerRotation,
                        serverPositionRotation2DViewComponent.ServerRotation)) > MathHelpers.EPSILON)
                serverPositionRotation2DViewComponent.Dirty = true;
            
            
            serverPositionRotation2DViewComponent.Position = position2DComponent.Position;
            
            serverPositionRotation2DViewComponent.Rotation = uniformRotationComponent.Angle;
        }
    }
}
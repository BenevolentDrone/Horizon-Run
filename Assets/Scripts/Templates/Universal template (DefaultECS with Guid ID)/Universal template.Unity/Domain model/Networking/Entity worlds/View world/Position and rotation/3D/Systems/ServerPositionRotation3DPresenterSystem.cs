using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class ServerPositionRotation3DPresenterSystem : AEntitySetSystem<float>
    {
        public ServerPositionRotation3DPresenterSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<ServerPositionRotation3DPresenterComponent>()
                    .With<ServerPositionRotation3DViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var serverPositionRotation3DPresenterComponent = entity.Get<ServerPositionRotation3DPresenterComponent>();

            ref var serverPositionRotation3DViewComponent = ref entity.Get<ServerPositionRotation3DViewComponent>();


            var targetEntity = serverPositionRotation3DPresenterComponent.TargetEntity;

            if (!targetEntity.IsAlive)
            {
                serverPositionRotation3DViewComponent.Visible = false;
                
                return;
            }
            
            serverPositionRotation3DViewComponent.Visible = true;


            var serverPosition3DComponent = targetEntity.Get<ServerPosition3DComponent>();
            
            var serverQuaternionComponent = targetEntity.Get<ServerQuaternionComponent>();
            
            var position3DComponent = targetEntity.Get<Position3DComponent>();
            
            var quaternionComponent = targetEntity.Get<QuaternionComponent>();

            
            var lastServerPosition = serverPositionRotation3DViewComponent.ServerPosition;
            
            serverPositionRotation3DViewComponent.ServerPosition = serverPosition3DComponent.ServerPosition;

            if ((lastServerPosition - serverPositionRotation3DViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
                serverPositionRotation3DViewComponent.Dirty = true;
            
            
            var lastServerRotation = serverPositionRotation3DViewComponent.ServerRotation;
            
            serverPositionRotation3DViewComponent.ServerRotation = serverQuaternionComponent.ServerQuaternion;

            float angle = Quaternion.Angle(
                lastServerRotation,
                serverPositionRotation3DViewComponent.ServerRotation);

            if (angle > Quaternion.kEpsilon) //MathHelpers.EPSILON)
                serverPositionRotation3DViewComponent.Dirty = true;
            
            
            serverPositionRotation3DViewComponent.Position = position3DComponent.Position;
            
            serverPositionRotation3DViewComponent.Rotation = quaternionComponent.Quaternion;
        }
    }
}
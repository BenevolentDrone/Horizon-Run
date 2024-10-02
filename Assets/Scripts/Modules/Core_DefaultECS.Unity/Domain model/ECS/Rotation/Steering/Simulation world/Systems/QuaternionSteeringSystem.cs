using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class QuaternionSteeringSystem : AEntitySetSystem<float>
    {
        public QuaternionSteeringSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<QuaternionComponent>()
                    .With<QuaternionSteeringComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var quaternionComponent = ref entity.Get<QuaternionComponent>();

            var quaternionSteeringComponent = entity.Get<QuaternionSteeringComponent>();

            float angle = Quaternion.Angle(
                quaternionComponent.Quaternion,
                quaternionSteeringComponent.TargetQuaternion);
            
            if (angle < Quaternion.kEpsilon) //MathHelpers.EPSILON)
                return;

            float progress = quaternionSteeringComponent.AngularSpeed * deltaTime / angle;

            progress = Mathf.Clamp01(progress);
            
            quaternionComponent.Quaternion = Quaternion.Slerp(
                quaternionComponent.Quaternion,
                quaternionSteeringComponent.TargetQuaternion,
                progress);
        }
    }
}
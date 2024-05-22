using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
    public class Locomotion3DSystem : AEntitySetSystem<float>
    {
        public Locomotion3DSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<PositionComponent>()
                    .With<Locomotion3DComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var positionComponent = ref entity.Get<PositionComponent>();

            var locomotion3DComponent = entity.Get<Locomotion3DComponent>();


            if (locomotion3DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
            {
                return;
            }
            
            float speed = locomotion3DComponent.LocomotionSpeedNormal * locomotion3DComponent.MaxLocomotionSpeed;

            Vector3 locomotionVector = 
                locomotion3DComponent.LocomotionVectorNormalized
                * (speed * deltaTime);

            positionComponent.Position += locomotionVector;
        }
    }
}
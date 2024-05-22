using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
    public class TransformQuaternionViewSystem : AEntitySetSystem<float>
    {
        public TransformQuaternionViewSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<TransformQuaternionViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var transformQuaternionViewComponent = ref entity.Get<TransformQuaternionViewComponent>();

            if (!transformQuaternionViewComponent.Dirty)
            {
                return;
            }

            transformQuaternionViewComponent.QuaternionPivotTransform.rotation =
                transformQuaternionViewComponent.Quaternion;

            transformQuaternionViewComponent.Dirty = false;
        }
    }
}
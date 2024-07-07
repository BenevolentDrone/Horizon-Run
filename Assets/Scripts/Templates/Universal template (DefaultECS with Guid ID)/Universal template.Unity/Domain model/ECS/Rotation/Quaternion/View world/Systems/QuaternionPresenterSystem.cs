using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class QuaternionPresenterSystem : AEntitySetSystem<float>
    {
        public QuaternionPresenterSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<QuaternionPresenterComponent>()
                    .With<TransformQuaternionViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var quaternionPresenterComponent = entity.Get<QuaternionPresenterComponent>();

            ref var transformQuaternionViewComponent = ref entity.Get<TransformQuaternionViewComponent>();


            var targetEntity = quaternionPresenterComponent.TargetEntity;

            if (!targetEntity.IsAlive)
            {
                return;
            }


            var quaternionComponent = targetEntity.Get<QuaternionComponent>();

            var lastQuaternion = transformQuaternionViewComponent.Quaternion;

            transformQuaternionViewComponent.Quaternion = quaternionComponent.Quaternion;

            /*
            if (Quaternion.Angle(
                lastQuaternion,
                transformQuaternionViewComponent.Quaternion)
                > MathHelpers.EPSILON)
                transformQuaternionViewComponent.Dirty = true;
            */

            //Courtesy of https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Quaternion.cs
            /*
            if (Quaternion.Dot(
                lastQuaternion,
                quaternionComponent.Quaternion)
                <= (1f - Quaternion.kEpsilon))
                transformQuaternionViewComponent.Dirty = true;
            */

            transformQuaternionViewComponent.Dirty = true;
        }
    }
}
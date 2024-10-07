using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class TransformPosition2DPresenterSystem : AEntitySetSystem<float>
	{
		public TransformPosition2DPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Position2DPresenterComponent>()
					.With<TransformPosition2DViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var positionPresenterComponent = entity.Get<Position2DPresenterComponent>();

			ref var transformPositionViewComponent = ref entity.Get<TransformPosition2DViewComponent>();


			var targetEntity = positionPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			var position2DComponent = targetEntity.Get<Position2DComponent>();

			var lastPosition = transformPositionViewComponent.Position;

			transformPositionViewComponent.Position = position2DComponent.Position;

			if ((lastPosition - transformPositionViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
				transformPositionViewComponent.Dirty = true;
		}
	}
}
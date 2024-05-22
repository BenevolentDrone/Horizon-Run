using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class TransformPositionPresenterSystem : AEntitySetSystem<float>
	{
		public TransformPositionPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<PositionPresenterComponent>()
					.With<TransformPositionViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var positionPresenterComponent = entity.Get<PositionPresenterComponent>();

			ref var transformPositionViewComponent = ref entity.Get<TransformPositionViewComponent>();


			var targetEntity = positionPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			var positionComponent = targetEntity.Get<PositionComponent>();

			var lastPosition = transformPositionViewComponent.Position;

			transformPositionViewComponent.Position = positionComponent.Position;

			if ((lastPosition - transformPositionViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
				transformPositionViewComponent.Dirty = true;
		}
	}
}
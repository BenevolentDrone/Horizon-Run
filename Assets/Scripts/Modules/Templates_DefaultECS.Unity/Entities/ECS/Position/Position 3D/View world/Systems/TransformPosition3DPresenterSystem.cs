using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class TransformPosition3DPresenterSystem : AEntitySetSystem<float>
	{
		public TransformPosition3DPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Position3DPresenterComponent>()
					.With<TransformPosition3DViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var positionPresenterComponent = entity.Get<Position3DPresenterComponent>();

			ref var transformPositionViewComponent = ref entity.Get<TransformPosition3DViewComponent>();


			var targetEntity = positionPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			var position3DComponent = targetEntity.Get<Position3DComponent>();

			var lastPosition = transformPositionViewComponent.Position;

			transformPositionViewComponent.Position = position3DComponent.Position;

			if ((lastPosition - transformPositionViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
				transformPositionViewComponent.Dirty = true;
		}
	}
}
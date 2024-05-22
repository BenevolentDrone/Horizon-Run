using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class TransformPositionViewSystem : AEntitySetSystem<float>
	{
		public TransformPositionViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<TransformPositionViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformPositionViewComponent = ref entity.Get<TransformPositionViewComponent>();

			if (!transformPositionViewComponent.Dirty)
			{
				return;
			}

			transformPositionViewComponent.PositionTransform.position = transformPositionViewComponent.Position;

			transformPositionViewComponent.Dirty = false;
		}
	}
}
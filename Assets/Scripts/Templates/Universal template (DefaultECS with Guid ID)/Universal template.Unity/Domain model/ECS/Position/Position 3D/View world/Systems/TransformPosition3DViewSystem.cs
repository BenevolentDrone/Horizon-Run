using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class TransformPosition3DViewSystem : AEntitySetSystem<float>
	{
		public TransformPosition3DViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<TransformPosition3DViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformPositionViewComponent = ref entity.Get<TransformPosition3DViewComponent>();

			if (!transformPositionViewComponent.Dirty)
			{
				return;
			}

			transformPositionViewComponent.PositionTransform.position = transformPositionViewComponent.Position;

			transformPositionViewComponent.Dirty = false;
		}
	}
}
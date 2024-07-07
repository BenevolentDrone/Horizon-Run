using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class TransformPosition2DViewSystem : AEntitySetSystem<float>
	{
		public TransformPosition2DViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<TransformPosition2DViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformPositionViewComponent = ref entity.Get<TransformPosition2DViewComponent>();

			if (!transformPositionViewComponent.Dirty)
			{
				return;
			}

			transformPositionViewComponent.PositionTransform.position = MathHelpersUnity.Vector2XZTo3(transformPositionViewComponent.Position);

			transformPositionViewComponent.Dirty = false;
		}
	}
}
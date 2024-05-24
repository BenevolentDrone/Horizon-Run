using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class DebugDrawLineViewSystem : AEntitySetSystem<float>
	{
		public DebugDrawLineViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<DebugDrawLineViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var debugDrawLineViewComponent = entity.Get<DebugDrawLineViewComponent>();

			if (!debugDrawLineViewComponent.Draw)
			{
				return;
			}

			UnityEngine.Debug.DrawLine(
				debugDrawLineViewComponent.SourcePosition,
				debugDrawLineViewComponent.TargetPosition,
				debugDrawLineViewComponent.Color);
		}
	}
}
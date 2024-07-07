using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class WASDControlsPresenterModelValidationSystem : AEntitySetSystem<float>
	{
		private EntitySet selectionSet;

		public WASDControlsPresenterModelValidationSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<WASDControlsPresenterComponent>()
					.AsSet())
		{
			selectionSet = world
				.GetEntities()
				.With<InputTargetSelectionComponent>()
				.AsSet();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var wasdControlsPresenterComponent = ref entity.Get<WASDControlsPresenterComponent>();

			//Update this all the time. Otherwise if selection changes and the previous selection target is still alive then we wont change the presenter's target at all
			/*
			var targetEntity = wasdControlsPresenterComponent.TargetEntity;

			if (targetEntity.IsAlive)
			{
				return;
			}
			*/

			foreach (var selectionEntity in selectionSet.GetEntities())
			{
				if (!selectionEntity.IsAlive)
				{
					continue;
				}

				wasdControlsPresenterComponent.TargetEntity = selectionEntity.Get<InputTargetSelectionComponent>().TargetEntity;

				return;
			}
		}
	}
}
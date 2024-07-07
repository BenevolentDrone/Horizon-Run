using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class JoystickPresenterModelValidationSystem : AEntitySetSystem<float>
	{
		private EntitySet selectionSet;

		public JoystickPresenterModelValidationSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<JoystickPresenterComponent>()
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
			ref var joystickPresenterComponent = ref entity.Get<JoystickPresenterComponent>();

			//Update this all the time. Otherwise if selection changes and the previous selection target is still alive then we wont change the presenter's target at all
			/*
			var targetEntity = joystickPresenterComponent.TargetEntity;

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

				joystickPresenterComponent.TargetEntity = selectionEntity.Get<InputTargetSelectionComponent>().TargetEntity;

				return;
			}
		}
	}
}
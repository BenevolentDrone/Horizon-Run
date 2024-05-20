using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleJoystickPresenterSystem : AEntitySetSystem<float>
	{
		public SampleJoystickPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleJoystickPresenterComponent>()
					.With<SampleJoystickViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleJoystickPresenterComponent = ref entity.Get<SampleJoystickPresenterComponent>();

			ref var sampleJoystickViewComponent = ref entity.Get<SampleJoystickViewComponent>();


			var targetEntity = sampleJoystickPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			if (targetEntity.Has<SampleLocomotionComponent>())
			{
				ref var sampleLocomotionComponent = ref targetEntity.Get<SampleLocomotionComponent>();

				sampleLocomotionComponent.LocomotionVector = sampleJoystickViewComponent.Joystick.Direction;
			}
		}
	}
}
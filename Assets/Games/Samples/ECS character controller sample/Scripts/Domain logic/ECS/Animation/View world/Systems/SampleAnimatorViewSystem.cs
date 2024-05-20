using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleAnimatorViewSystem : AEntitySetSystem<float>
	{
		private readonly ILogger logger;

		public SampleAnimatorViewSystem(
			World world,
			ILogger logger = null)
			: base(
				world
					.GetEntities()
					.With<SampleAnimatorViewComponent>()
					.AsSet())
		{
			this.logger = logger;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleAnimatorViewComponent = ref entity.Get<SampleAnimatorViewComponent>();

			if (sampleAnimatorViewComponent.Animator == null)
			{
				logger?.LogError<SampleAnimatorViewSystem>(
					"ANIMATOR IS NULL",
					new object[] { this });

				return;
			}

			bool running = sampleAnimatorViewComponent.LocomotionVector.magnitude > MathHelpers.EPSILON;

			bool talking = false;

			bool idle =
				!running
				&& !talking;

			sampleAnimatorViewComponent.Animator.SetBool(
				sampleAnimatorViewComponent.IdleParameter,
				idle);

			sampleAnimatorViewComponent.Animator.SetBool(
				sampleAnimatorViewComponent.RunningParameter,
				running);

			sampleAnimatorViewComponent.Animator.SetBool(
				sampleAnimatorViewComponent.TalkingParameter,
				talking);

			if (idle && !sampleAnimatorViewComponent.WasIdleLastUpdate)
			{
				sampleAnimatorViewComponent.Animator.SetInteger(
					sampleAnimatorViewComponent.IdleAnimationSelectionParameter,
					UnityEngine.Random.Range(0, sampleAnimatorViewComponent.IdleAnimationsCount));
			}

			sampleAnimatorViewComponent.WasIdleLastUpdate = idle;
		}
	}
}
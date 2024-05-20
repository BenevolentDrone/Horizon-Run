using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[ViewComponent]
	public class SampleAnimatorViewComponent : AMonoViewComponent
	{
		public Animator Animator;

		public string IdleParameter;

		public string RunningParameter;

		public string TalkingParameter;

		public string IdleAnimationSelectionParameter;

		public int IdleAnimationsCount;

		public bool WasIdleLastUpdate = true;

		public Vector2 LocomotionVector;
	}
}
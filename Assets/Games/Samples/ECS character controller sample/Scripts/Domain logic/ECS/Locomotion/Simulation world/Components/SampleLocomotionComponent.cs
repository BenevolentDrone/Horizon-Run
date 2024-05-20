using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Component("Simulation world/Locomotion")]
	public struct SampleLocomotionComponent
	{
		public float MaxLocomotionSpeed;

		public Vector2 LocomotionVector;
	}
}
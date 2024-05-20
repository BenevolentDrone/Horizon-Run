using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Component("Simulation world/Locomotion")]
	public struct SampleLocomotionMemoryComponent
	{
		public Vector2 LastLocomotionVector;
	}
}
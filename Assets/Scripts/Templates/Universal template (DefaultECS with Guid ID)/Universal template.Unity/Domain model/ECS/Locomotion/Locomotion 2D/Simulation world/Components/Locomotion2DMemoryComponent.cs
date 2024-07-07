using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Locomotion")]
	public struct Locomotion2DMemoryComponent
	{
		public Vector2 LastLocomotionVectorNormalized;
	}
}
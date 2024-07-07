using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Locomotion")]
	public struct Locomotion3DMemoryComponent
	{
		public Vector3 LastLocomotionVectorNormalized;
	}
}
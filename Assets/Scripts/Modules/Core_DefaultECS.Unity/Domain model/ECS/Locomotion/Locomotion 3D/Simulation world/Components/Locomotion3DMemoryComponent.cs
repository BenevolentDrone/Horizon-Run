using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Locomotion")]
	public struct Locomotion3DMemoryComponent
	{
		public Vector3 LastLocomotionVectorNormalized;
	}
}
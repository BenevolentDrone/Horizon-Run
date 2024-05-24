using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Locomotion")]
	public struct Locomotion3DMemoryComponent
	{
		public Vector3 LastLocomotionVectorNormalized;
	}
}
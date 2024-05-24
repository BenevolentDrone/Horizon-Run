using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Locomotion")]
	public struct Locomotion2DMemoryComponent
	{
		public Vector2 LastLocomotionVectorNormalized;
	}
}
using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Vehicle")]
	public struct SuspensionComponent
	{
		public Vector3 SuspensionDirectionNormalized;

		public float SuspensionLength;

		public float MaxSuspensionLength;

		
	}
}
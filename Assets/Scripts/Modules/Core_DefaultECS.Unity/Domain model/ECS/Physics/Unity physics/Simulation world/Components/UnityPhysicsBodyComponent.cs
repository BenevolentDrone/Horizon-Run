using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Physics")]
	public struct UnityPhysicsBodyComponent
	{
		public ushort PhysicsBodyHandle;
	}
}
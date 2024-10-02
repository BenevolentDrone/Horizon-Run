using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Physics")]
	public struct Gravity3DComponent
	{
		public Vector3 Gravity;

		public bool Enabled;
	}
}
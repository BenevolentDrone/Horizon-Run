using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Vehicle")]
	public struct Wheel3DComponent
	{
		public float Radius;

		public float Width;

		public Vector3 Force;
	}
}
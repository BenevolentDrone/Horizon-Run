using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Vehicle")]
	public struct Wheel3DComponent
	{
		public float Radius;

		public float Width;

		public Vector3 Force;
	}
}
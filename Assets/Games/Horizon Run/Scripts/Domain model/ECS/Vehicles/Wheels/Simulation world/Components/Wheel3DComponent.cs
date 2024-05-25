using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Vehicle")]
	public struct Wheel3DComponent
	{
		public float Radius;

		public float Width;

		public Vector3 Force;
	}
}
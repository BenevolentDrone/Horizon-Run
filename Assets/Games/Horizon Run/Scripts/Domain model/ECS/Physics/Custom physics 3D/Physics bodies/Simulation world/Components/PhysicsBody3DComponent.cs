using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Physics")]
	public struct PhysicsBody3DComponent
	{
		public Vector3 LinearVelocity;

		public Vector3 AngularVelocity;

		//Drags
		public float LinearDragScalar;

		public float AngularDragScalar;
	}
}
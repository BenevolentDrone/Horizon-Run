using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Physics")]
	public struct PhysicsBody3DComponent
	{
		//Current frame forces and torques
		//They convert into velocities and angular velocities
		public Vector3 ForceThisFrame;

		public Vector3 TorqueThisFrame;

		//Current frame push forces and torques
		//They convert into positions and rotations
		public Vector3 ConstraintForceThisFrame;

		public Vector3 ConstraintTorqueThisFrame;

		//Velocities
		public Vector3 LinearVelocity;

		public Vector3 AngularVelocity;

		//Drags
		public float LinearDragScalar;

		public float AngularDragScalar;
	}
}
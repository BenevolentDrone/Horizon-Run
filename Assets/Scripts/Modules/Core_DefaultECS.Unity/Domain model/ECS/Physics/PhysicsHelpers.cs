using HereticalSolutions.Entities;

using DefaultEcs;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public static class PhysicsHelpers
	{
		public static void AddForce(
			Vector3 force,
			float timeDelta,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.ForceThisFrame += force * timeDelta;
		}

		public static void AddTorque(
			Vector3 torque,
			float timeDelta,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.TorqueThisFrame += torque * timeDelta;
		}

		public static void AddImpulseForce(
			Vector3 force,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.ForceThisFrame += force;
		}

		public static void AddImpulseTorque(
			Vector3 torque,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.TorqueThisFrame += torque;
		}

		public static void AddConstraintForce(
			Vector3 force,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.ConstraintForceThisFrame += force;
		}

		public static void AddConstraintTorque(
			Vector3 torque,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.ConstraintTorqueThisFrame += torque;
		}

		public static void ApplyForceAt(
			Vector3 force,
			Vector3 position,
			Vector3 centreOfMass,
			ref PhysicsBody3DComponent physicsBody3DComponent)
		{
			physicsBody3DComponent.ForceThisFrame += force;

			physicsBody3DComponent.TorqueThisFrame += Vector3.Cross(
				position - centreOfMass,
				force);
		}
	}
}
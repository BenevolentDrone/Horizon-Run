using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class Wheel3DSystem : AEntitySetSystem<float>
	{
		private readonly RaycastHit[] raycastHits;

		private readonly LayerMask layerMask;

		//private readonly float castDistance;

		public Wheel3DSystem(
			World world,
			RaycastHit[] raycastHits,
			LayerMask layerMask)
			: base(
				world
					.GetEntities()
					.With<Wheel3DComponent>()
					.With<PhysicsBody3DComponent>()
					.With<Transform3DComponent>()
					.With<Suspension3DComponent>()
					.AsSet())
		{
			this.raycastHits = raycastHits;

			this.layerMask = layerMask;

			//this.castDistance = castDistance;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var wheelComponent = ref entity.Get<Wheel3DComponent>();

			ref var transformComponent = ref entity.Get<Transform3DComponent>();

			ref var suspensionComponent = ref entity.Get<Suspension3DComponent>();

			Vector3 wheelWorldPosition = TransformHelpers.GetWorldPosition3D(
				transformComponent.TRSMatrix);

			var wheelWorldRotation = transformComponent.TRSMatrix.rotation;

			//FUCK https://gamedev.stackexchange.com/questions/163682/why-is-my-spherecast-returning-an-incorrect-hit-on-mesh-collider
			//YOU https://forum.unity.com/threads/spherecastall-returns-0-0-0-for-all-raycasthit-points.428302/
			//UNITY https://discussions.unity.com/t/physics-spherecastnonalloc-returning-incorrect-information/204102

			//int hits = Physics.SphereCastNonAlloc(
			//	wheelWorldPosition,
			//	wheelComponent.Radius,
			//	-Vector3.up,
			//	raycastHits,
			//	castDistance,
			//	layerMask);

			Vector3 wheelUp = wheelWorldRotation * Vector3.up;

			Vector3 wheelRight = wheelWorldRotation * Vector3.right;

			var ray = new Ray(
				wheelWorldPosition + wheelUp * (wheelComponent.Radius * 2f),
				-wheelUp);

			int hits = Physics.SphereCastNonAlloc(
				ray,
				wheelComponent.Radius,
				raycastHits,
				wheelComponent.Radius * 2f,
				layerMask);

			if (hits == 0)
			{
				suspensionComponent.SuspensionAttachmentReceivesForce = true;

				suspensionComponent.SuspensionJointReceivesForce = false;

				return;
			}

			Vector3 force = Vector3.zero;

			for (int i = 0; i < hits; i++)
			{
				//The hit is not a guaranteed 'hit'. Let's check whether it is actually within the wheel
				Vector3 contact = raycastHits[i].point;

				Vector3 directionToHit = contact - wheelWorldPosition;

				//Step 1: if it's outside the wheel 'capsule' then we discard it
				if (directionToHit.magnitude > wheelComponent.Radius)
				{
					UnityEngine.Debug.DrawLine(
						contact,
						contact + raycastHits[i].normal * 0.1f,
						Color.red);

					continue;
				}

				//Step 2: if it's now within the width of the wheel then we discard it
				var contactPerpendicularDot = Vector3.Dot(
					directionToHit,
					wheelRight);

				if (Mathf.Abs(contactPerpendicularDot) > (wheelComponent.Width * 0.5f))
				{
					UnityEngine.Debug.DrawLine(
						contact,
						contact + raycastHits[i].normal * 0.1f,
						Color.magenta);

					continue;
				}

				//Wheel landed onto something, the spring shall provide force to the vehicle instead of the wheel
				suspensionComponent.SuspensionAttachmentReceivesForce = false;

				suspensionComponent.SuspensionJointReceivesForce = true;

				UnityEngine.Debug.DrawLine(
					contact,
					contact + raycastHits[i].normal * 0.1f,
					Color.green);

				float penetrationScalar = wheelComponent.Radius - directionToHit.magnitude;

				force += raycastHits[i].normal * penetrationScalar;
			}

			wheelComponent.Force = force;

			UnityEngine.Debug.DrawLine(
				wheelWorldPosition,
				wheelWorldPosition + force,
				Color.red);


			//Apply forces
			ref var physicsBodyComponent = ref entity.Get<PhysicsBody3DComponent>();

			PhysicsHelpers.AddConstraintForce(
				wheelComponent.Force,
				ref physicsBodyComponent);
		}
	}
	*/
}
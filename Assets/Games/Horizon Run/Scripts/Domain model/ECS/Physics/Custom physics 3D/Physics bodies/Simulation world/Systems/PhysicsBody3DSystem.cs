using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class PhysicsBody3DSystem : AEntitySetSystem<float>
	{
		public PhysicsBody3DSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Transform3DComponent>()
					.With<Position3DComponent>()
					.With<QuaternionComponent>()
					.With<PhysicsBody3DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformComponent = ref entity.Get<Transform3DComponent>();

			ref var positionComponent = ref entity.Get<Position3DComponent>();

			ref var quaternionComponent = ref entity.Get<QuaternionComponent>();

			ref var physicsBodyComponent = ref entity.Get<PhysicsBody3DComponent>();

			//Courtesy of https://github.com/notgiven688/jitterphysics2/blob/main/src/Jitter2/World.Step.cs
			/*
			rigidBody.AngularVelocity *= body.angularDampingMultiplier;
			rigidBody.Velocity *= body.linearDampingMultiplier;

			rigidBody.DeltaVelocity = body.Force * rigidBody.InverseMass * substep_dt;
			rigidBody.DeltaAngularVelocity = JVector.Transform(body.Torque, rigidBody.InverseInertiaWorld) * substep_dt;

			if (body.AffectedByGravity)
			{
				rigidBody.DeltaVelocity += gravity * substep_dt;
			}

			body.Force = JVector.Zero;
			body.Torque = JVector.Zero;

			var bodyOrientation = JMatrix.CreateFromQuaternion(rigidBody.Orientation);

			JMatrix.Multiply(bodyOrientation, body.inverseInertia, out rigidBody.InverseInertiaWorld);
			JMatrix.MultiplyTransposed(rigidBody.InverseInertiaWorld, bodyOrientation, out rigidBody.InverseInertiaWorld);

			rigidBody.InverseMass = body.inverseMass;
			*/


			ApplyDampingToLinearVelocity(
				ref physicsBodyComponent.LinearVelocity,
				physicsBodyComponent.LinearDragScalar);

			//ApplyDampingToAngularVelocity(
			//	ref physicsBodyComponent.AngularVelocity,
			//	physicsBodyComponent.AngularDragScalar);

			//Courtesy of https://github.com/notgiven688/jitterphysics2/blob/main/src/Jitter2/World.Step.cs
			/*
			JVector lvel = rigidBody.Velocity;
			JVector avel = rigidBody.AngularVelocity;

			rigidBody.Position += lvel * substep_dt;

			float angle = avel.Length();
			JVector axis;

			if (angle < 0.001f)
			{
				// use Taylor's expansions of sync function
				// axis = body.angularVelocity * (0.5f * timestep - (timestep * timestep * timestep) * (0.020833333333f) * angle * angle);
				JVector.Multiply(avel,
					0.5f * substep_dt - substep_dt * substep_dt * substep_dt * 0.020833333333f * angle * angle,
					out axis);
			}
			else
			{
				// sync(fAngle) = sin(c*fAngle)/t
				JVector.Multiply(avel, (float)Math.Sin(0.5f * angle * substep_dt) / angle, out axis);
			}

			JQuaternion dorn = new(axis.X, axis.Y, axis.Z, (float)Math.Cos(angle * substep_dt * 0.5f));
			//JQuaternion.CreateFromMatrix(rigidBody.Orientation, out JQuaternion ornA);
			JQuaternion ornA = rigidBody.Orientation;

			JQuaternion.Multiply(dorn, ornA, out dorn);

			dorn.Normalize();
			//JMatrix.CreateFromQuaternion(dorn, out rigidBody.Orientation);
			rigidBody.Orientation = dorn;
			*/

			UpdatePosition(
				ref positionComponent.Position,
				physicsBodyComponent.LinearVelocity,
				deltaTime);

			//UpdateRotation(
			//	ref quaternionComponent.Quaternion,
			//	physicsBodyComponent.AngularVelocity,
			//	deltaTime);

			transformComponent.Dirty = true;
		}

		private void ApplyDampingToLinearVelocity(
			ref Vector3 linearVelocity,
			float linearDragScalar)
		{
			//float linearVelocityMagnitude = linearVelocity.magnitude;
//
			//float dampenedLinearVelocityMagnitude = Mathf.Abs(linearVelocityMagnitude - linearDragScalar);
//
			//linearVelocity = linearVelocity.normalized * dampenedLinearVelocityMagnitude;


			linearVelocity *= (1f - linearDragScalar);
		}

		private void ApplyDampingToAngularVelocity(
			ref Vector3 angularVelocity,
			float angularDragScalar)
		{
			//float angularVelocityMagnitude = angularVelocity.magnitude;
//
			//float dampenedAngularVelocityMagnitude = Mathf.Abs(angularVelocityMagnitude - angularDragScalar);
//
			//angularVelocity = angularVelocity.normalized * dampenedAngularVelocityMagnitude;


			angularVelocity *= (1f - angularDragScalar);
		}

		private void UpdatePosition(
			ref Vector3 position,
			Vector3 velocity,
			float deltaTime)
		{
			position += velocity * deltaTime;
		}

		private void UpdateRotation(
			ref Quaternion rotation,
			Vector3 angularVelocity,
			float deltaTime)
		{
			float angleMagnitude = angularVelocity.magnitude;

			Vector3 axis = angularVelocity
				* (Mathf.Sin(0.5f * angleMagnitude * deltaTime) / angleMagnitude);

			Quaternion deltaRotation = new Quaternion(
				axis.x,
				axis.y,
				axis.z,
				Mathf.Cos(angleMagnitude * deltaTime * 0.5f));

			rotation = deltaRotation * rotation;
		}
	}
}
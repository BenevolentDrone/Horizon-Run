using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
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

			ref var position3DComponent = ref entity.Get<Position3DComponent>();

			ref var quaternionComponent = ref entity.Get<QuaternionComponent>();

			ref var physicsBodyComponent = ref entity.Get<PhysicsBody3DComponent>();

			//DEBUG
			var worldPosition = TransformHelpers.GetWorldPosition3D(
				transformComponent.TRSMatrix);


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

			if (physicsBodyComponent.LinearDragScalar > MathHelpers.EPSILON)
			{
				ApplyDampingToLinearVelocity(
					ref physicsBodyComponent.LinearVelocity,
					physicsBodyComponent.LinearDragScalar,
					deltaTime);
			}

			if (physicsBodyComponent.AngularDragScalar > MathHelpers.EPSILON)
			{
				ApplyDampingToAngularVelocity(
					ref physicsBodyComponent.AngularVelocity,
					physicsBodyComponent.AngularDragScalar,
					deltaTime);
			}

			/*
			rigidBody.AngularVelocity += rigidBody.DeltaAngularVelocity;

			rigidBody.Velocity += rigidBody.DeltaVelocity;
			*/


			if (physicsBodyComponent.ConstraintForceThisFrame.magnitude > MathHelpers.EPSILON)
			{
				ApplyConstraintForceToPosition(
					physicsBodyComponent.ConstraintForceThisFrame,
					ref position3DComponent.Position);

				UnityEngine.Debug.DrawLine(
					worldPosition,
					worldPosition + physicsBodyComponent.LinearVelocity,
					Color.yellow);

				var dot = Vector3.Dot(
					physicsBodyComponent.LinearVelocity,
					physicsBodyComponent.ConstraintForceThisFrame.normalized);

				if (Mathf.Abs(dot) < physicsBodyComponent.ConstraintForceThisFrame.magnitude)
				{
					UnityEngine.Debug.DrawLine(
						worldPosition,
						worldPosition + physicsBodyComponent.ConstraintForceThisFrame,
						Color.red);
	
					UnityEngine.Debug.DrawLine(
						worldPosition,
						worldPosition + physicsBodyComponent.ConstraintForceThisFrame.normalized * Mathf.Abs(dot),
						Color.blue);
				}
				else
				{
					UnityEngine.Debug.DrawLine(
						worldPosition,
						worldPosition + physicsBodyComponent.ConstraintForceThisFrame.normalized * Mathf.Abs(dot),
						Color.blue);

					UnityEngine.Debug.DrawLine(
						worldPosition,
						worldPosition + physicsBodyComponent.ConstraintForceThisFrame,
						Color.red);
				}

				ApplyConstraintForceToLinearVelocity(
					physicsBodyComponent.ConstraintForceThisFrame,
					ref physicsBodyComponent.LinearVelocity);

				physicsBodyComponent.ConstraintForceThisFrame = Vector3.zero;
			}

			if (physicsBodyComponent.ConstraintTorqueThisFrame.magnitude > MathHelpers.EPSILON)
			{
				ApplyConstraintTorqueToRotation(
					physicsBodyComponent.ConstraintTorqueThisFrame,
					ref quaternionComponent.Quaternion);

				ApplyConstraintTorqueToAngularVelocity(
					physicsBodyComponent.ConstraintTorqueThisFrame,
					ref physicsBodyComponent.AngularVelocity);

				physicsBodyComponent.ConstraintTorqueThisFrame = Vector3.zero;
			}

			//ApplyPushForceToPosition(
			//	physicsBodyComponent.PushForceThisFrame,
			//	ref position3DComponent.Position);

			//ApplyPushTorqueToRotation(
			//	physicsBodyComponent.PushTorqueThisFrame,
			//	ref quaternionComponent.Quaternion);

			//ApplyPushForceToLinearVelocity(
			//	physicsBodyComponent.PushForceThisFrame,
			//	ref physicsBodyComponent.LinearVelocity);

			//ApplyPushTorqueToAngularVelocity(
			//	physicsBodyComponent.PushTorqueThisFrame,
			//	ref physicsBodyComponent.AngularVelocity);

			physicsBodyComponent.ConstraintForceThisFrame = Vector3.zero;

			physicsBodyComponent.ConstraintTorqueThisFrame = Vector3.zero;


			if (physicsBodyComponent.ForceThisFrame.magnitude > MathHelpers.EPSILON)
			{
				ApplyForceToLinearVelocity(
					physicsBodyComponent.ForceThisFrame,
					ref physicsBodyComponent.LinearVelocity);
			}

			if (physicsBodyComponent.TorqueThisFrame.magnitude > MathHelpers.EPSILON)
			{
				ApplyTorqueToAngularVelocity(
					physicsBodyComponent.TorqueThisFrame,
					ref physicsBodyComponent.AngularVelocity);
			}

			physicsBodyComponent.ForceThisFrame = Vector3.zero;

			physicsBodyComponent.TorqueThisFrame = Vector3.zero;

			if (physicsBodyComponent.LinearVelocity.magnitude > MathHelpers.EPSILON)
			{
				ApplyLinearVelocityToPosition(
					physicsBodyComponent.LinearVelocity,
					ref position3DComponent.Position,
					deltaTime);
			}

			if (physicsBodyComponent.AngularVelocity.magnitude > MathHelpers.EPSILON)
			{
				ApplyAngularVelocityToRotation(
					physicsBodyComponent.AngularVelocity,
					ref quaternionComponent.Quaternion,
					deltaTime);
			}

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

			//var worldPosition = TransformHelpers.GetWorldPosition3D(
			//	transformComponent.TRSMatrix);

			UnityEngine.Debug.DrawLine(
				worldPosition,
				worldPosition + physicsBodyComponent.LinearVelocity,
				Color.green);

			transformComponent.Dirty = true;
		}

		private void ApplyDampingToLinearVelocity(
			ref Vector3 linearVelocity,
			float linearDragScalar,
			float deltaTime)
		{
			linearVelocity *= (1f - linearDragScalar * deltaTime);
		}

		private void ApplyDampingToAngularVelocity(
			ref Vector3 angularVelocity,
			float angularDragScalar,
			float deltaTime)
		{
			angularVelocity *= (1f - angularDragScalar * deltaTime);
		}

		private void ApplyConstraintForceToPosition(
			Vector3 force,
			ref Vector3 position)
		{
			position += force;
		}

		private void ApplyConstraintTorqueToRotation(
			Vector3 torque,
			ref Quaternion rotation)
		{
			Vector3 halfAngle = torque * (0.5f);

			float l = halfAngle.magnitude;

			if (l > 0)
			{
				halfAngle *= Mathf.Sin(l) / l;
			}

			Quaternion deltaRotation = new Quaternion(
				halfAngle.x,
				halfAngle.y,
				halfAngle.z,
				Mathf.Cos(l));

			//Because I got sick of errors like 
			//"Quaternion To Matrix conversion failed because input Quaternion is invalid {bla bla bla} l={totally not 1}"
			rotation = (deltaRotation * rotation).normalized;
		}

		private void ApplyConstraintForceToLinearVelocity(
			Vector3 force,
			ref Vector3 linearVelocity)
		{
			var dot = Vector3.Dot(
				linearVelocity,
				force.normalized);

			linearVelocity += force.normalized * Mathf.Abs(dot);
		}

		private void ApplyConstraintTorqueToAngularVelocity(
			Vector3 torque,
			ref Vector3 angularVelocity)
		{
			var dot = Vector3.Dot(
				angularVelocity,
				torque.normalized);

			angularVelocity += torque.normalized * Mathf.Abs(dot);
		}

		private void ApplyForceToLinearVelocity(
			Vector3 force,
			ref Vector3 linearVelocity)
		{
			linearVelocity += force;
		}

		private void ApplyTorqueToAngularVelocity(
			Vector3 torque,
			ref Vector3 angularVelocity)
		{
			angularVelocity += torque;
		}

		private void ApplyLinearVelocityToPosition(
			Vector3 velocity,
			ref Vector3 position,
			float deltaTime)
		{
			position += velocity * deltaTime;
		}

		private void ApplyAngularVelocityToRotation(
			Vector3 angularVelocity,
			ref Quaternion rotation,
			float deltaTime)
		{
			Vector3 halfAngle = angularVelocity * (deltaTime * 0.5f);

			float l = halfAngle.magnitude;

			if (l > 0)
			{
				halfAngle *= Mathf.Sin(l) / l;
			}

			Quaternion deltaRotation = new Quaternion(
				halfAngle.x,
				halfAngle.y,
				halfAngle.z,
				Mathf.Cos(l));

			//Because I got sick of errors like 
			//"Quaternion To Matrix conversion failed because input Quaternion is invalid {bla bla bla} l={totally not 1}"
			rotation = (deltaRotation * rotation).normalized;
		}
	}
}
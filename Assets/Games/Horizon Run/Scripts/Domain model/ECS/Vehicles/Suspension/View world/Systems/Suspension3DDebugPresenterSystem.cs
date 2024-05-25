using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Suspension3DDebugPresenterSystem : AEntitySetSystem<float>
	{
		public Suspension3DDebugPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Suspension3DPresenterComponent>()
					.With<Suspension3DDebugViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var suspension3DPresenterComponent = entity.Get<Suspension3DPresenterComponent>();

			ref var suspension3DDebugViewComponent = ref entity.Get<Suspension3DDebugViewComponent>();


			var targetEntity = suspension3DPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}

			var transformComponent = targetEntity.Get<Transform3DComponent>();

			var suspensionComponent = targetEntity.Get<Suspension3DComponent>();

			var parentTRSMatrix = TransformHelpers.GetParentTRSMatrix(entity);

			//Get spring vector
			Vector3 springAttachmentWorldPosition = TransformHelpers.GetWorldPosition3D(
				transformComponent.TRSMatrix);

			Vector3 springJointWorldPosition = TransformHelpers.GetWorldPosition3D(
				suspensionComponent.JointPosition,
				parentTRSMatrix);

			Vector3 springVector = springAttachmentWorldPosition - springJointWorldPosition;

			//Get suspension direction rotated
			Vector3 suspensionDirectionNormalizedRotated = TransformHelpers.GetWorldPosition3D(
				suspensionComponent.SuspensionDirectionNormalized,
				Quaternion.identity,
				Vector3.one,
				parentTRSMatrix);

			//Get current spring length
			float currentSpringLength = Vector3.Dot(
				springVector,
				suspensionDirectionNormalizedRotated);

			//Calculate spring force. Spring force is directed in the direction that allows the spring to be back at rest length again
			//Positive compression means the spring is too long, negative means it's too short
			float springCompression = currentSpringLength - suspensionComponent.RestLength;

			float springCompressionNormalized = springCompression / suspensionComponent.TravelLength;

			//Compression cannot go beyond the travel length - the rest is a longitudal error
			float springCompressionNormalizedClamped = Mathf.Clamp(
				springCompressionNormalized,
				-1f,
				1f);

			//-1 for an opposite direction
			float springForceScalar = -1f * springCompressionNormalizedClamped * suspensionComponent.Stiffness;

			Vector3 springForce = suspensionDirectionNormalizedRotated * springForceScalar;

			//Calculate the damping force
			//-1 for an opposite direction
			float springDampingForceScalar = -1f * suspensionComponent.Damping * springForceScalar;

			Vector3 springDampingForce = suspensionDirectionNormalizedRotated * springDampingForceScalar;

			//Calculate suspension longitudinal error. Error is directed in the direction that allows the spring to be within the travel length again
			//Positive error means the spring is too long, negative means it's too short
			Vector3 suspensionLongitudinalError = Vector3.zero;

			float suspensionLongitudinalErrorScalar = Mathf.Abs(springCompression) - suspensionComponent.TravelLength;

			if (suspensionLongitudinalErrorScalar > 0f)
			{
				//Spring can go over the travel length in both directions
				//For the if() we took an abs() to see whether the longitudal error actually exists
				suspensionLongitudinalErrorScalar *= Mathf.Sign(springCompression);

				//-1 for an opposite direction
				suspensionLongitudinalErrorScalar *= -1f;

				suspensionLongitudinalError = suspensionDirectionNormalizedRotated
					* suspensionLongitudinalErrorScalar;
			}

			//Calculate suspension latteral error
			Vector3 expectedSuspensionWorldPosition =
				springJointWorldPosition
				+ suspensionDirectionNormalizedRotated * currentSpringLength;

			Vector3 suspensionLatteralError = springAttachmentWorldPosition - expectedSuspensionWorldPosition;

			//Calculate forces
			Vector3 forceAppliedToSuspensionAttachment = Vector3.zero;

			Vector3 forceAppliedToSuspensionJoint = Vector3.zero;

			//Dunno. Maybe there's a case like that?
			if (suspensionComponent.SuspensionAttachmentReceivesForce
				&& suspensionComponent.SuspensionJointReceivesForce)
			{
				//-1 for an opposite direction. The compiler will optimize const values anyway
				forceAppliedToSuspensionJoint = (-1f * 0.5f) * (springForce + springDampingForce + suspensionLongitudinalError);

				//minus for an opposite direction
				forceAppliedToSuspensionAttachment = 0.5f * (springForce + springDampingForce + suspensionLongitudinalError) - suspensionLatteralError;
			}
			//Wheel is in the air
			else if (suspensionComponent.SuspensionAttachmentReceivesForce)
			{
				//minus for an opposite direction
				forceAppliedToSuspensionAttachment = springForce + springDampingForce + suspensionLongitudinalError - suspensionLatteralError;
			}
			//Wheel is on the ground
			else if (suspensionComponent.SuspensionJointReceivesForce)
			{
				//-1 for an opposite direction
				forceAppliedToSuspensionJoint = -1f * (springForce + springDampingForce + suspensionLongitudinalError);

				//minus for an opposite direction
				forceAppliedToSuspensionAttachment = -suspensionLatteralError;
			}


			suspension3DDebugViewComponent.SuspensionJointPosition = springJointWorldPosition;

			suspension3DDebugViewComponent.SuspensionAttachmentPosition = springAttachmentWorldPosition;


			suspension3DDebugViewComponent.SuspensionRestPosition =
				springJointWorldPosition + suspensionDirectionNormalizedRotated * suspensionComponent.RestLength;

			suspension3DDebugViewComponent.SuspensionCompressionMinPosition =
				springJointWorldPosition + suspensionDirectionNormalizedRotated * (suspensionComponent.RestLength - suspensionComponent.TravelLength);

			suspension3DDebugViewComponent.SuspensionCompressionMaxPosition =
				springJointWorldPosition + suspensionDirectionNormalizedRotated * (suspensionComponent.RestLength + suspensionComponent.TravelLength);


			suspension3DDebugViewComponent.ForceAppliedToJoint = forceAppliedToSuspensionJoint;

			suspension3DDebugViewComponent.ForceAppliedToAttachment = forceAppliedToSuspensionAttachment;


			suspension3DDebugViewComponent.SuspensionLatteralError = suspensionLatteralError;

			suspension3DDebugViewComponent.SpringCompressionNormalized = springCompressionNormalized;
		}
	}
}
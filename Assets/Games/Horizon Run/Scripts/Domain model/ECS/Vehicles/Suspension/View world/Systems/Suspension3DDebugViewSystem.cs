using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Suspension3DDebugViewSystem : AEntitySetSystem<float>
	{
		private readonly Vector3 padding = new Vector3(0f, 0f, 0.05f);

		public Suspension3DDebugViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Suspension3DDebugViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var suspension3DDebugViewComponent = entity.Get<Suspension3DDebugViewComponent>();

			//Draw spring the way it would be in the rest
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionJointPosition + padding,
				suspension3DDebugViewComponent.SuspensionRestPosition + padding,
				Color.grey);

			//Draw spring travel length
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionCompressionMinPosition + padding * 2f,
				suspension3DDebugViewComponent.SuspensionCompressionMaxPosition + padding * 2f,
				Color.yellow);

			//Visualize the compression with a color lerp
			Color suspensionColor = Color.Lerp(
				Color.white,
				Color.red,
				Mathf.Abs(suspension3DDebugViewComponent.SpringCompressionNormalized));

			//Draw spring the way it is now
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionJointPosition,
				suspension3DDebugViewComponent.SuspensionAttachmentPosition,
				suspensionColor);

			//Draw suspension longitudal error
			//Debug.DrawLine(
			//	suspension3DDebugViewComponent.SuspensionPosition,
			//	suspension3DDebugViewComponent.SuspensionPosition + suspension3DDebugViewComponent.SuspensionError,
			//	Color.blue);

			//Draw suspension latteral error
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionAttachmentPosition - padding,
				suspension3DDebugViewComponent.SuspensionAttachmentPosition + suspension3DDebugViewComponent.SuspensionLatteralError - padding,
				Color.red);

			//Draw the force applied to suspension joint
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionJointPosition - padding,
				suspension3DDebugViewComponent.SuspensionJointPosition + suspension3DDebugViewComponent.ForceAppliedToJoint - padding,
				Color.green);

			//Draw the force applied to suspension attachment
			Debug.DrawLine(
				suspension3DDebugViewComponent.SuspensionAttachmentPosition - padding,
				suspension3DDebugViewComponent.SuspensionAttachmentPosition + suspension3DDebugViewComponent.ForceAppliedToAttachment - padding,
				Color.blue);
		}
	}
}
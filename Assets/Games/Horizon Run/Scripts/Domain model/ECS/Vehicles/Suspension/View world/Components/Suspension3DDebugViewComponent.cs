using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("View world/Debug")]
	public struct Suspension3DDebugViewComponent
	{
		public Vector3 SuspensionJointPosition;

		public Vector3 SuspensionAttachmentPosition;

		public Vector3 SuspensionRestPosition;

		public Vector3 SuspensionCompressionMinPosition;

		public Vector3 SuspensionCompressionMaxPosition;

		public Vector3 ForceAppliedToJoint;

		public Vector3 ForceAppliedToAttachment;

		public Vector3 SuspensionLatteralError;

		public float SpringCompressionNormalized;
	}
}
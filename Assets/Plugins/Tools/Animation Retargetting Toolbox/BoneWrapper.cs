using UnityEngine;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class BoneWrapper
	{
		public BoneWrapper(
			string boneName,
			Transform boneTransform,
			BoneSnapshot poseSnapshot)
		{
			BoneName = boneName;

			BoneTransform = boneTransform;

			PoseSnapshot = poseSnapshot;

			SanitationSnapshot = new BoneSnapshot
			{
				Position = Vector3.zero,
				Rotation = Quaternion.identity,
				BoneMode = EBoneMode.ADDITIVE
			};
		}

		public string BoneName { get; private set; }

		public Transform BoneTransform { get; private set; }

		public BoneSnapshot PoseSnapshot { get; set; }

		public BoneSnapshot SanitationSnapshot { get; set; }
	}
}
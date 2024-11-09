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
		}

		public string BoneName { get; private set; }

		public Transform BoneTransform { get; private set; }

		public BoneSnapshot PoseSnapshot { get; private set; }
	}
}
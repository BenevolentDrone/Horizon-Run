using HereticalSolutions.Repositories;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class PoseSnapshot
	{
		private readonly IReadOnlyRepository<BoneWrapper, BoneSnapshot> boneSnapshots;

		public PoseSnapshot(
			IReadOnlyRepository<BoneWrapper, BoneSnapshot> boneSnapshots)
		{
			this.boneSnapshots = boneSnapshots;
		}

		public IReadOnlyRepository<BoneWrapper, BoneSnapshot> BoneSnapshots { get => boneSnapshots; }
	}
}
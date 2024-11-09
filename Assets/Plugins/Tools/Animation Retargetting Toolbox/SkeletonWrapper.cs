using HereticalSolutions.Hierarchy;

using UnityEngine;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class SkeletonWrapper
	{
		public SkeletonWrapper(
			GameObject skeletonGameObject,
			IHierarchyNode<BoneWrapper> rootNode)
		{
			SkeletonGameObject = skeletonGameObject;

			RootNode = rootNode;
		}

		public GameObject SkeletonGameObject { get; set; }

		public IHierarchyNode<BoneWrapper> RootNode { get; set; }
	}
}
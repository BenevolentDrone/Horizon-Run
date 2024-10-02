using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	//Note that this is NOT a component
	public struct LocalTransform3D
	{
		public Vector3 LocalPosition;

		public Quaternion LocalRotation;

		public Vector3 LocalScale;
	}
}
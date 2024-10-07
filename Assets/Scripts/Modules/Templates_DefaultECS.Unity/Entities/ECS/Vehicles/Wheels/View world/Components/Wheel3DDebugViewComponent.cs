using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Debug")]
	public struct Wheel3DDebugViewComponent
	{
		public Vector3 WheelPosition;

		public Quaternion WheelRotation;

		public float WheelRadius;

		public float WheelWidth;
	}
}
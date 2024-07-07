using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
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
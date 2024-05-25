using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Vehicle")]
	public struct Suspension3DComponent
	{
		//Where is suspension attached at and which direction it is facing
		public LocalTransform3D JointPosition;

		public Vector3 SuspensionDirectionNormalized;

		//Actual suspension properties
		//Courtesy of https://www.youtube.com/watch?v=sWshRRDxdSU
		public float RestLength;

		public float TravelLength;

		public float Stiffness;
		public float Damping;

		//Results of suspension calcilations
		public Vector3 SuspensionForce;

		public Vector3 SuspensionError;

		//Tells which entities receive forces
		public bool SuspensionAttachmentReceivesForce;

		public bool SuspensionJointReceivesForce;
	}
}
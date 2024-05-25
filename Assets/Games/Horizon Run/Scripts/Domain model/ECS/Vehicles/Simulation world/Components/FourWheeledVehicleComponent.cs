using System;

using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Vehicles")]
	public struct FourWheeledVehicleComponent
	{
		public Guid FrontRightWheel;

		public Vector3 FrontRightWheelJointPosition;

		public Guid RearRightWheel;

		public Vector3 RearRightWheelJointPosition;

		public Guid RearLeftWheel;

		public Vector3 RearLeftWheelJointPosition;

		public Guid FrontLeftWheel;

		public Vector3 FrontLeftWheelJointPosition;
	}
}
using System;

using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Vehicle")]
	public struct FourWheeledVehicleComponent
	{
		public Guid FrontRightWheel;

		public Vector3 FrontRightWheelSocketPosition;

		public Guid RearRightWheel;

		public Vector3 RearRightWheelSocketPosition;

		public Guid RearLeftWheel;

		public Vector3 RearLeftWheelSocketPosition;

		public Guid FrontLeftWheel;

		public Vector3 FrontLeftWheelSocketPosition;
	}
}
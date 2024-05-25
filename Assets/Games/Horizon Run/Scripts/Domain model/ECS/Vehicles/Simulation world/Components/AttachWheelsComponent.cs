using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[InitializationCommandComponent]
	[Component("Simulation world/Vehicles")]
	public struct AttachWheelsComponent
	{
		public string WheelPrototypeID;

		public float DesiredSuspensionRestLength;

		public float DesiredSuspensionTravelLength;

		public float DesiredSuspensionStiffness;

		public float DesiredSuspensionDamping;
	}
}
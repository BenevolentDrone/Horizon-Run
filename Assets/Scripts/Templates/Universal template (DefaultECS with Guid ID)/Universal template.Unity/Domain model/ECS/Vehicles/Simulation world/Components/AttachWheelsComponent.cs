using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
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
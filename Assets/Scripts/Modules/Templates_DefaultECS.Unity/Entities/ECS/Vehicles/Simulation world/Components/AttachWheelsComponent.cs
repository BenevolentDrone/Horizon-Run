using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
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
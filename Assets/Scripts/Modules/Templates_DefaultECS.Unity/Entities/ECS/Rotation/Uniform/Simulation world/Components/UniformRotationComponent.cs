using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Rotation")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct UniformRotationComponent
	{
		public float Angle;
	}
}
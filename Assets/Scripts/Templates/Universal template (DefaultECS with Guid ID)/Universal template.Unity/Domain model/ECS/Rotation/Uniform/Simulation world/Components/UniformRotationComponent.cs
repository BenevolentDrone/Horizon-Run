using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Rotation")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct UniformRotationComponent
	{
		public float Angle;
	}
}
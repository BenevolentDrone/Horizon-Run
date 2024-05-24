using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Rotation")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct UniformRotationComponent
	{
		public float Angle;
	}
}
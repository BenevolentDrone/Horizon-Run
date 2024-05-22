using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Rotation")]
	public struct UniformSteeringComponent
	{
		public float TargetAngle;

		public float AngularSpeed;
	}
}
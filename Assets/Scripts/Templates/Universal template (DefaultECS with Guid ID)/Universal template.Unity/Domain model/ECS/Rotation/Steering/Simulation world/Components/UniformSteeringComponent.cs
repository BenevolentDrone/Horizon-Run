using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Rotation")]
	public struct UniformSteeringComponent
	{
		public float TargetAngle;

		public float AngularSpeed;
	}
}
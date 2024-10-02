using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Rotation")]
	public struct UniformSteeringComponent
	{
		public float TargetAngle;

		public float AngularSpeed;
	}
}
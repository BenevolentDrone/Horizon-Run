using HereticalSolutions.Entities;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Component("Simulation world/Rotation")]
	public struct SampleSteeringComponent
	{
		public float TargetAngle;

		public float AngularSpeed;
	}
}
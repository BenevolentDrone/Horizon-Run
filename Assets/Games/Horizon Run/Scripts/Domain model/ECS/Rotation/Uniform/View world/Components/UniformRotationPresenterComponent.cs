using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	[Component("View world/Presenters")]
	public struct UniformRotationPresenterComponent
	{
		public Entity TargetEntity;
	}
}
using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public interface IDefaultECSEntityWorldController
		: IWorldController<
			World,
			IDefaultECSEntityInitializationSystem,
			Entity>
	{
	}
}
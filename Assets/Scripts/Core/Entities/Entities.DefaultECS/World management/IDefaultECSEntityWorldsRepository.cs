using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public interface IDefaultECSEntityWorldsRepository
		: IEntityWorldsRepository<
			World,
			IDefaultECSEntityWorldController>
	{
	}
}
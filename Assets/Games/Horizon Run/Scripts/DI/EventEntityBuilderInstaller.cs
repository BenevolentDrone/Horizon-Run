using System;

using HereticalSolutions.Entities;
using HereticalSolutions.Entities.Factories;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
	public class EventEntityBuilderInstaller : MonoInstaller
	{
		[Inject]
		private HorizonRunEntityManager entityManager;

		public override void InstallBindings()
		{
			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldsRepository = worldContainer.EntityWorldsRepository;

			var eventWorld = entityWorldsRepository.GetWorld(WorldConstants.EVENT_WORLD_ID);

			var eventEntityBuilder = DefaultECSEntityFactory.BuildDefaultECSEventEntityBuilder<Guid>(
				eventWorld);

			Container
				.Bind<IEventEntityBuilder<Entity, Guid>>()
				.FromInstance(eventEntityBuilder)
				.AsCached();
		}
	}
}
using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class EventEntityBuilderInstaller : MonoInstaller
	{
		[Inject]
		private EntityWorldRepository entityWorldRepository;

		public override void InstallBindings()
		{
			if (!entityWorldRepository.TryGetWorld(
				WorldConstants.EVENT_WORLD_ID,
				out var eventWorld))
			{
				//TODO: Log error

				return;
			}

			var eventEntityBuilder = EntityFactory.BuildEventEntityBuilder<Guid>(
				eventWorld);

			Container
				.Bind<IEventEntityBuilder<Entity, Guid>>()
				.FromInstance(eventEntityBuilder)
				.AsCached();
		}
	}
}
using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Entities
{
	public class EventProcessedSystem<TEventComponent, TDelta>
		: AEntitySetSystem<TDelta>
	{
		public EventProcessedSystem(World eventWorld)
			: base(
				eventWorld
					.GetEntities()
					.With<TEventComponent>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
		}

		protected override void Update(TDelta delta, in Entity entity)
		{
			entity.Set<EventProcessedComponent>();
		}
	}
}
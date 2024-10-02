using System;

using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class LMBClickPresenterSystem : AEntitySetSystem<float>
	{
		private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		public LMBClickPresenterSystem(
			World world,
            IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
			: base(
				world
					.GetEntities()
					.With<LMBClickPresenterComponent>()
					.With<LMBClickViewComponent>()
					.AsSet())
		{
			this.eventEntityBuilder = eventEntityBuilder;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{

			ref var LMBClickSelectionViewComponent = ref entity.Get<LMBClickViewComponent>();

			if (!LMBClickSelectionViewComponent.Dirty)
				return;

			if (LMBClickSelectionViewComponent.EntityClicked)
			{
				var targetEntity = LMBClickSelectionViewComponent.ViewEntityClicked;

				if(!targetEntity.IsAlive)
					return;
				
				eventEntityBuilder
					.NewEvent(out var eventEntity)
					.AddressedToWorldLocalEntity(
						eventEntity,
						targetEntity)
					.WithData<EntityInteractedWithByPlayerEventComponent>(
						eventEntity,
						new EntityInteractedWithByPlayerEventComponent
						{
							InteractionMode = LMBClickSelectionViewComponent.InteractionMode
						});

				LMBClickSelectionViewComponent.EntityClicked = false;
				
				LMBClickSelectionViewComponent.ViewEntityClicked = default;

				LMBClickSelectionViewComponent.InteractionMode = default;
			}
			
			if (LMBClickSelectionViewComponent.TerrainClicked)
			{
				eventEntityBuilder
					.NewEvent(out var eventEntity)
					.TargetedAtPosition(
						eventEntity,
						new System.Numerics.Vector3(
							LMBClickSelectionViewComponent.TerrainPositionClicked.x,
							LMBClickSelectionViewComponent.TerrainPositionClicked.y,
							LMBClickSelectionViewComponent.TerrainPositionClicked.z))
					.WithData<TerrainInteractedWithByPlayerEventComponent>(
						eventEntity,
						new TerrainInteractedWithByPlayerEventComponent
						{
							InteractionMode = LMBClickSelectionViewComponent.InteractionMode
						});

				LMBClickSelectionViewComponent.TerrainClicked = false;
                
				LMBClickSelectionViewComponent.TerrainPositionClicked = default;
				
				LMBClickSelectionViewComponent.InteractionMode = default;
			}

			LMBClickSelectionViewComponent.Dirty = false;
		}
	}
}
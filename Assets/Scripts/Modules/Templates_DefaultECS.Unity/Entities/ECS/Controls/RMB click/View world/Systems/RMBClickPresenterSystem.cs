using System;

using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class RMBClickPresenterSystem : AEntitySetSystem<float>
    {
        private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

        public RMBClickPresenterSystem(
            World world,
            IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
            : base(
                world
                    .GetEntities()
                    .With<RMBClickPresenterComponent>()
                    .With<RMBClickViewComponent>()
                    .AsSet())
        {
            this.eventEntityBuilder = eventEntityBuilder;
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var RMBClickOrderIssuingViewComponent = ref entity.Get<RMBClickViewComponent>();

            if (!RMBClickOrderIssuingViewComponent.Dirty)
                return;

            if (RMBClickOrderIssuingViewComponent.EntityClicked)
            {
                var targetEntity = RMBClickOrderIssuingViewComponent.ViewEntityClicked;

                eventEntityBuilder
                    .NewEvent(out var eventEntity)
                    .AddressedToWorldLocalEntity(
                        eventEntity,
                        targetEntity)
                    .WithData<EntityInteractedWithByPlayerEventComponent>(
                        eventEntity,
                        new EntityInteractedWithByPlayerEventComponent
                        {
                            InteractionMode = EInteractionMode.RMB_CLICK
                        });
                
                RMBClickOrderIssuingViewComponent.ViewEntityClicked = default;

                RMBClickOrderIssuingViewComponent.EntityClicked = false;
            }
            
            if (RMBClickOrderIssuingViewComponent.TerrainClicked)
            {
                if (!RMBClickOrderIssuingViewComponent.TerrainClickHeld
                    || RMBClickOrderIssuingViewComponent.ShouldKeepIssuingMoveOrderOnMouseHold)
                {
                    eventEntityBuilder
                        .NewEvent(out var eventEntity)
                        .TargetedAtPosition(
                            eventEntity,
                            new System.Numerics.Vector3(
                                RMBClickOrderIssuingViewComponent.TerrainPositionClicked.x,
                                RMBClickOrderIssuingViewComponent.TerrainPositionClicked.y,
                                RMBClickOrderIssuingViewComponent.TerrainPositionClicked.z))
                        .WithData<TerrainInteractedWithByPlayerEventComponent>(
                            eventEntity,
                            new TerrainInteractedWithByPlayerEventComponent
                            {
                                InteractionMode = EInteractionMode.RMB_CLICK
                            });
                    
                    RMBClickOrderIssuingViewComponent.TerrainClickHeld = true;
                }


                if (!RMBClickOrderIssuingViewComponent.ClickEffectPlayed
                    && RMBClickOrderIssuingViewComponent.ClickEffectParticleSystem != null
                    && RMBClickOrderIssuingViewComponent.ClickEffectTransform != null)
                {
                    RMBClickOrderIssuingViewComponent
                        .ClickEffectTransform
                        .transform
                        .position = RMBClickOrderIssuingViewComponent.TerrainPositionClicked;
                    
                    RMBClickOrderIssuingViewComponent.ClickEffectParticleSystem.Play();

                    RMBClickOrderIssuingViewComponent.ClickEffectPlayed = true;
                }
                
                
                RMBClickOrderIssuingViewComponent.TerrainClicked = false;
                
                RMBClickOrderIssuingViewComponent.TerrainPositionClicked = default;
            }

            RMBClickOrderIssuingViewComponent.Dirty = false;
        }
    }
}
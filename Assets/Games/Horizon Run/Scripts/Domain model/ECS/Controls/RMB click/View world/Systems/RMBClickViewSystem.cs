using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    public class RMBClickViewSystem : AEntitySetSystem<float>
    {
        private readonly Camera mainRenderCamera;

        private readonly IUIManager uiManager;

        public RMBClickViewSystem(
            World world,
            Camera mainRenderCamera,
            IUIManager uiManager)
            : base(
                world
                    .GetEntities()
                    .With<RMBClickViewComponent>()
                    .AsSet())
        {
            this.mainRenderCamera = mainRenderCamera;

            this.uiManager = uiManager;
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var RMBClickOrderIssuingViewComponent = ref entity.Get<RMBClickViewComponent>();

            if (Input.GetMouseButtonDown(1))
            {
                if (uiManager.HUDHovered)
                {
                    return;
                }

                var ray = mainRenderCamera.ScreenPointToRay(Input.mousePosition);

                var hits = Physics.RaycastAll(
                    ray,
                    Mathf.Infinity,
                    RMBClickOrderIssuingViewComponent.InteractableMask);
                
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hit = hits[i];
                        
                        var viewEntityReference = hit.collider.GetComponent<ViewEntityReferenceBehaviour>();

                        if (viewEntityReference == null)
                            continue;

                        var viewAdapter = viewEntityReference.ViewAdapter;

                        var viewEntity = viewAdapter.ViewEntity;

                        if (viewEntity.IsAlive)
                        {
                            RMBClickOrderIssuingViewComponent.EntityClicked = true;

                            RMBClickOrderIssuingViewComponent.ViewEntityClicked = viewEntity;

                            RMBClickOrderIssuingViewComponent.Dirty = true;
                            
                            break;
                        }
                    }
                }
                else if (Physics.Raycast(
                    ray,
                    out var terrainHit,
                    Mathf.Infinity,
                    RMBClickOrderIssuingViewComponent.TerrainMask))
                {
                    RMBClickOrderIssuingViewComponent.TerrainClicked = true;
                    
                    RMBClickOrderIssuingViewComponent.ClickEffectPlayed = false;
                    
                    RMBClickOrderIssuingViewComponent.TerrainClickReleased = false;
                        
                    RMBClickOrderIssuingViewComponent.TerrainPositionClicked = terrainHit.point;

                    RMBClickOrderIssuingViewComponent.Dirty = true;
                }
            }

            if (Input.GetMouseButton(1)
                && !RMBClickOrderIssuingViewComponent.TerrainClicked        //to ensure that we don't send the event twice
                && !RMBClickOrderIssuingViewComponent.TerrainClickReleased)
            {
                if (uiManager.HUDHovered)
                {
                    return;
                }

                var ray = mainRenderCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(
                    ray,
                    out var terrainHit,
                    Mathf.Infinity,
                    RMBClickOrderIssuingViewComponent.TerrainMask))
                {
                    RMBClickOrderIssuingViewComponent.TerrainClicked = true;
                    
                    RMBClickOrderIssuingViewComponent.TerrainPositionClicked = terrainHit.point;

                    RMBClickOrderIssuingViewComponent.Dirty = true;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                RMBClickOrderIssuingViewComponent.TerrainClickReleased = true;
                
                RMBClickOrderIssuingViewComponent.ClickEffectPlayed = false;
                
                RMBClickOrderIssuingViewComponent.TerrainClickHeld = false;
            }
        }
    }
}
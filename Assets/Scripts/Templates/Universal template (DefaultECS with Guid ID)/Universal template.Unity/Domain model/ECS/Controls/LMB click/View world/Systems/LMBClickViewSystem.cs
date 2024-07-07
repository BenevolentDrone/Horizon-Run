using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class LMBClickViewSystem : AEntitySetSystem<float>
	{
		private readonly Camera mainRenderCamera;

		private readonly IUIManager uiManager;

		public LMBClickViewSystem(
			World world,
			Camera mainRenderCamera,
			IUIManager uiManager)
			: base(
				world
					.GetEntities()
					.With<LMBClickViewComponent>()
					.AsSet())
		{
			this.mainRenderCamera = mainRenderCamera;

			this.uiManager = uiManager;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var LMBClickSelectionViewComponent = ref entity.Get<LMBClickViewComponent>();

            var ray = mainRenderCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
			{
				if (uiManager.HUDHovered)
				{
					LMBClickSelectionViewComponent.Dirty = true;
					
					return;
				}

				bool doubleClick = 
					(UnityEngine.Time.time - LMBClickSelectionViewComponent.LastClickTime)
					< LMBClickSelectionViewComponent.DoubleClickTimeout;
				
				var hits = Physics.RaycastAll(
					ray,
					Mathf.Infinity,
					LMBClickSelectionViewComponent.InteractableMask);

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
							LMBClickSelectionViewComponent.EntityClicked = true;

							LMBClickSelectionViewComponent.ViewEntityClicked = viewEntity;

							LMBClickSelectionViewComponent.InteractionMode = (doubleClick)
								? EInteractionMode.LMB_DOUBLE_CLICK
								: EInteractionMode.LMB_CLICK;

							LMBClickSelectionViewComponent.Dirty = true;
							
							break;
						}
					}
				}
				else if (Physics.Raycast(
					ray,
					out var terrainHit,
					Mathf.Infinity,
					LMBClickSelectionViewComponent.TerrainMask))
				{
					LMBClickSelectionViewComponent.TerrainClicked = true;
                        
					LMBClickSelectionViewComponent.TerrainPositionClicked = terrainHit.point;
					
					LMBClickSelectionViewComponent.InteractionMode = (doubleClick)
						? EInteractionMode.LMB_DOUBLE_CLICK
						: EInteractionMode.LMB_CLICK;

					LMBClickSelectionViewComponent.Dirty = true;
				}

				LMBClickSelectionViewComponent.LastClickTime = UnityEngine.Time.time;
			}
		}
	}
}
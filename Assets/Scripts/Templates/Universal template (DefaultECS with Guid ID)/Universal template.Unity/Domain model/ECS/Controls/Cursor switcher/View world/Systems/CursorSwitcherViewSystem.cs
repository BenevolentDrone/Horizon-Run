using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class CursorSwitcherViewSystem : AEntitySetSystem<float>
	{
		public CursorSwitcherViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<CursorSwitcherViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var cursorSwitcherViewComponent = ref entity.Get<CursorSwitcherViewComponent>();
			
			if (!cursorSwitcherViewComponent.Dirty)
				return;
			
			if (string.IsNullOrEmpty(cursorSwitcherViewComponent.PreferredCursor))
				return;

			foreach (var data in cursorSwitcherViewComponent.Cursors)
			{
				if (data.Name == cursorSwitcherViewComponent.PreferredCursor)
				{
					cursorSwitcherViewComponent.CursorShapesAsset.Arrow.Texture = data.Texture;
					
					cursorSwitcherViewComponent.CursorShapesAsset.Arrow.Hotspot = data.Hotspot;

					Cursor.SetCursor(
						data.Texture,
						data.Hotspot,
						CursorMode.Auto);
					
					break;
				}
			}
			
			cursorSwitcherViewComponent.Dirty = false;
		}
	}
}
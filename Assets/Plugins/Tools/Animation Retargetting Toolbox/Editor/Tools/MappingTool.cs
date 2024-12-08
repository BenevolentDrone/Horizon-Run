using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Hierarchy;

using UnityEngine;

using UnityEditor;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class MappingTool
		: IARToolboxToolWindow
	{
		#region Constants

		#region UI consts

		private const string UI_TEXT_TITLE = "Mapping tool";

		private const string UI_TEXT_MAP_LOWERCASE = "map";

		private const string UI_TEXT_MAP_CAMELCASE = "Map";

		#endregion

		#region Metadata keys

		private const string KEY_TOOL_PREFIX = "MappingTool";

		private const string KEY_MAPS = "MappingTool_Maps";

		private const string KEY_MAP_PREFIX = "Map";

		#endregion

		#endregion

		#region IARToolboxToolWindow

		public void Draw(IARToolboxContext context)
		{
			ARToolboxEditorHelpers.BeginInnerBoxWithTitle(
				UI_TEXT_TITLE);

			bool enabled = ARToolboxEditorHelpers.BeginEnabledCheck(
				context,
				KEY_TOOL_PREFIX);

			if (enabled)
			{
				var maps = ARToolboxEditorHelpers.GetSubcontextList(
					context,
					KEY_MAPS,
					UI_TEXT_MAP_LOWERCASE);

				for (int i = 0; i < maps.Count; i++)
				{
					DrawMap(
						maps[i],
						i,
						maps.Count);
				}

				ARToolboxEditorHelpers.UpdateSubcontextList(
					maps,
					KEY_MAP_PREFIX);
			}

			ARToolboxEditorHelpers.EndInnerBox();
		}

		public void DrawHandles(IARToolboxContext context)
		{
		}

		public void SceneUpdate(IARToolboxContext context)
		{
		}

		#endregion

		private void DrawMap(
			IARToolboxContext context,
			int mapIndex,
			int totalMapsCount)
		{
			bool enabled = ARToolboxEditorHelpers.BeginSubcontextBoxWithTitle(
				context,
				KEY_MAP_PREFIX,
				UI_TEXT_MAP_CAMELCASE,
				mapIndex);

			if (!enabled)
			{
				ARToolboxEditorHelpers.DrawSubcontextControls(
					context,
					mapIndex,
					totalMapsCount,
					KEY_MAP_PREFIX);

				ARToolboxEditorHelpers.EndSubcontextBox();

				return;
			}

			ARToolboxEditorHelpers.DrawSubcontextControls(
				context,
				mapIndex,
				totalMapsCount,
				KEY_MAP_PREFIX);

			ARToolboxEditorHelpers.EndSubcontextBox();
		}
	}
}
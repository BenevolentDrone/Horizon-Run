using UnityEngine;

using System.Collections.Generic;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class DearImGuiManager
		: MonoBehaviour,
		  IUIManager,
		  IDearImGuiManager
	{
		private List<object> hoveredWindows = new List<object>();

        public bool HUDHovered
		{ 
			get
			{
				return hoveredWindows.Count > 0;
			} 
		}

        public void RegisterHoveredImGuiWindow(object window)
		{
			if (!hoveredWindows.Contains(window))
			{
				hoveredWindows.Add(window);
			}
		}

		public void UnregisterHoveredImGuiWindow(object window)
		{
			if (hoveredWindows.Contains(window))
			{
				hoveredWindows.Remove(window);
			}
		}
    }
}
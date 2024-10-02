using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[ViewComponent]
	public class WASDControlsViewComponent : AMonoViewComponent
	{
		public Vector2 Direction;
	}
}
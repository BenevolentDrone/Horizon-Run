using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[ViewComponent]
	public class TransformPosition2DViewComponent : AMonoViewComponent
	{
		public Transform PositionTransform;

		public Vector2 Position;

		public bool Dirty;
	}
}
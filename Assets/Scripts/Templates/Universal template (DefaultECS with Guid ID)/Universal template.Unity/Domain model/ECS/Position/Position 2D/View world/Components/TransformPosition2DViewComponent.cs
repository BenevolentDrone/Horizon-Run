using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[ViewComponent]
	public class TransformPosition2DViewComponent : AMonoViewComponent
	{
		public Transform PositionTransform;

		public Vector2 Position;

		public bool Dirty;
	}
}
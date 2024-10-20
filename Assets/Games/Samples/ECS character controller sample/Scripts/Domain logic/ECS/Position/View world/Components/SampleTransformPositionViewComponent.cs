using HereticalSolutions.Modules.Core_DefaultECS;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[ViewComponent]
	public class SampleTransformPositionViewComponent : AMonoViewComponent
	{
		public Transform PositionTransform;

		public Vector2 Position;

		public bool Dirty;
	}
}
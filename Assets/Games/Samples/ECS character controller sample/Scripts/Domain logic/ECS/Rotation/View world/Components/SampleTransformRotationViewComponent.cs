using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[ViewComponent]
	public class SampleTransformRotationViewComponent : AMonoViewComponent
	{
		public Transform RotationPivotTransform;

		public float Angle;

		public bool Dirty;
	}
}
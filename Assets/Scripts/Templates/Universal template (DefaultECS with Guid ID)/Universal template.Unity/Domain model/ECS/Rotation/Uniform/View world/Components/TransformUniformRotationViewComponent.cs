using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[ViewComponent]
	public class TransformUniformRotationViewComponent : AMonoViewComponent
	{
		public Transform RotationPivotTransform;

		public float Angle;

		public bool Dirty;
	}
}
using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[ViewComponent]
	public class TransformUniformRotationViewComponent : AMonoViewComponent
	{
		public Transform RotationPivotTransform;

		public float Angle;

		public bool Dirty;
	}
}
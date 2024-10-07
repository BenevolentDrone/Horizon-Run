using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[ViewComponent]
	public class TransformUniformRotationViewComponent : AMonoViewComponent
	{
		public Transform RotationPivotTransform;

		public float Angle;

		public bool Dirty;
	}
}
using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[ViewComponent]
	public class TransformPositionViewComponent : AMonoViewComponent
	{
		public Transform PositionTransform;

		public Vector3 Position;

		public bool Dirty;
	}
}
using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[ViewComponent]
	public class TransformPosition3DViewComponent : AMonoViewComponent
	{
		public Transform PositionTransform;

		public Vector3 Position;

		public bool Dirty;
	}
}
using UnityEngine;

namespace HereticalSolutions.Pools
{
	public class ParentTransformArgument : IPoolPopArgument
	{
		public Transform Parent;

		public bool WorldPositionStays = true;
	}
}
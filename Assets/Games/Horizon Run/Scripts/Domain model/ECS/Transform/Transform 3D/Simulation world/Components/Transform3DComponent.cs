using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Transform")]
	public struct Transform3DComponent
	{
		public Matrix4x4 TRSMatrix;

		public bool Dirty;
	}
}
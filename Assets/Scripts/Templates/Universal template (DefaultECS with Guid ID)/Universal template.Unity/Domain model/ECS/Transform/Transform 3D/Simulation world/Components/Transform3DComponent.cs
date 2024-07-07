using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Transform")]
	public struct Transform3DComponent
	{
		public Matrix4x4 TRSMatrix;

		public bool Dirty;
	}
}
using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Debug")]
	public struct DebugDrawLineViewComponent
	{
		public Vector3 SourcePosition;

		public Vector3 TargetPosition;

		public Color Color;

		public bool Draw;
	}
}
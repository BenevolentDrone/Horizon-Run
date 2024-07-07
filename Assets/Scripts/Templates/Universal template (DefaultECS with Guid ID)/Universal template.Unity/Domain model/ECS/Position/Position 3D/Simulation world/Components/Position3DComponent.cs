using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Position")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct Position3DComponent
	{
		public Vector3 Position;
	}
}
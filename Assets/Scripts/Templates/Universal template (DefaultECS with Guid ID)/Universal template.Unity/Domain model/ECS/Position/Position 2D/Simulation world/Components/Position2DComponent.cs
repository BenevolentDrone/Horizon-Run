using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Position")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct Position2DComponent
	{
		public Vector2 Position;
	}
}
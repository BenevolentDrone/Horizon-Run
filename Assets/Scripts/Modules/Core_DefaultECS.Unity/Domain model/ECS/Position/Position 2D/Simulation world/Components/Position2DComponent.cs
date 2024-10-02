using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Position")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct Position2DComponent
	{
		public Vector2 Position;
	}
}
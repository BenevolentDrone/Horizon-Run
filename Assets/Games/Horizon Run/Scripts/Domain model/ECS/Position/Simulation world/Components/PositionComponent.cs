using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Position")]
	[ServerAuthoredOnInitializationComponent]
	[ServerAuthoredComponent]
	public struct PositionComponent
	{
		public Vector3 Position;
	}
}
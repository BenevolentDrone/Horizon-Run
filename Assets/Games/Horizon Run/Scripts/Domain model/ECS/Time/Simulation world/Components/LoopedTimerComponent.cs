using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Time")]
	[ClientDisabledComponent]
	public struct LoopedTimerComponent
	{
		public float Timeout;

		public ushort TimerHandle;
	}
}
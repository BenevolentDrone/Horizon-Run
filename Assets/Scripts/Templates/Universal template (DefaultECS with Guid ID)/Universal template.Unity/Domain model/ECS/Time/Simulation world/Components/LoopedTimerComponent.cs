using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Time")]
	[ClientDisabledComponent]
	public struct LoopedTimerComponent
	{
		public float Timeout;

		public ushort TimerHandle;
	}
}
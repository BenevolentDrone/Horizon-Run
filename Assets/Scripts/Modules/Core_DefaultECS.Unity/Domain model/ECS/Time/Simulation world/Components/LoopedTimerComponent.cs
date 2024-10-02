using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Time")]
	[ClientDisabledComponent]
	public struct LoopedTimerComponent
	{
		public float Timeout;

		public ushort TimerHandle;
	}
}
using System;

namespace HereticalSolutions.Networking
{
	[Serializable]
	public class NetworkTimeoutSettings
	{
		public int DisconnectTimeoutInMs = 50000;

		public float PickingTimeout = 60f;

		public float TimeoutBeforeStart = 20f;
	}
}
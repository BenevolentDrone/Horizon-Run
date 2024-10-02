using System;

namespace HereticalSolutions.Networking
{
	[Serializable]
	public class NetworkRollCallSettings
	{
		public ushort MaxEntityInfoRequestsPerRollCall = 5;

		public float DelayBetweenRepeatedEntityInfoRequests = 1;

		public ushort RollCallFrequency = 10;
	}
}
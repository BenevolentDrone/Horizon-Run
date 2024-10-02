using System;

namespace HereticalSolutions.Networking
{
	[Serializable]
	public class NetworkTargetFPSSettings
	{
		public int TargetServerFPSInStandby = 1;

		public int TargetServerFPSInGame = 30;
	}
}
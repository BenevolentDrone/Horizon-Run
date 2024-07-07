using System;

namespace HereticalSolutions.Networking
{
	[Serializable]
	public class NetworkPositionInterpolationSettings
	{
		public float PositionInterpolationThreshold = 0.05f;

		public float PositionInterpolationValue = 0.5f;

		public float PositionMaxAllowedDeviation = 0.2f;
	}
}
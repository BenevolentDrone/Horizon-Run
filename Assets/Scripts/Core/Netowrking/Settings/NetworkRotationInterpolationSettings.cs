using System;

namespace HereticalSolutions.Networking
{
	[Serializable]
	public class NetworkRotationInterpolationSettings
	{
		public float RotationInterpolationThreshold = 2f;

		public float RotationInterpolationValue = 0.5f;

		public float RotationMaxAllowedDeviation = 10f;
	}
}
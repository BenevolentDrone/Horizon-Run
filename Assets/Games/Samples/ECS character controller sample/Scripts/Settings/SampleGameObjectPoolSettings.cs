using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[CreateAssetMenu(fileName = "Sample game object pool settings", menuName = "Settings/Samples/Game object pools/Sample game object pool settings", order = 0)]
	public class SampleGameObjectPoolSettings : ScriptableObject
	{
		public string PoolID;

		public SampleGameObjectWithAddressSettings[] Elements;
	}
}
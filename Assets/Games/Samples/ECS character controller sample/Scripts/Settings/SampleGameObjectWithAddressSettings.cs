using System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Serializable]
	public class SampleGameObjectWithAddressSettings
	{
		public string GameObjectAddress;

		public SampleGameObjectVariantSettings[] Variants;
	}
}
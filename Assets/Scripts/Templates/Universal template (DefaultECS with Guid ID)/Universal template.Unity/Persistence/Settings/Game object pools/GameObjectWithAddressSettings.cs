using System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Serializable]
	public class GameObjectWithAddressSettings
	{
		public string GameObjectAddress;

		public GameObjectVariantSettings[] Variants;
	}
}
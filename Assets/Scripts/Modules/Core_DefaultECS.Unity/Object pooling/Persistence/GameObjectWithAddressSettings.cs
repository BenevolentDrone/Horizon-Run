using System;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[Serializable]
	public class GameObjectWithAddressSettings
	{
		public string GameObjectAddress;

		public GameObjectVariantSettings[] Variants;
	}
}
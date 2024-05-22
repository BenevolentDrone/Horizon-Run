using System;

namespace HereticalSolutions.HorizonRun
{
	[Serializable]
	public class GameObjectWithAddressSettings
	{
		public string GameObjectAddress;

		public GameObjectVariantSettings[] Variants;
	}
}
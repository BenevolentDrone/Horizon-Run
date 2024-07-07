using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[CreateAssetMenu(fileName = "Game object pool settings", menuName = "Settings/Game object pools/Game object pool settings", order = 0)]
	public class GameObjectPoolSettings : ScriptableObject
	{
		public string PoolID;

		public GameObjectWithAddressSettings[] Elements;
	}
}
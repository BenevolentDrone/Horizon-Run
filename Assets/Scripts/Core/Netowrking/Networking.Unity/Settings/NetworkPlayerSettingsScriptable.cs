using UnityEngine;

namespace HereticalSolutions.Networking.Unity
{
	[CreateAssetMenu(fileName = "Network player settings", menuName = "Settings/Network/Network player settings", order = 0)]
	public class NetworkPlayerSettingsScriptable : ScriptableObject
	{
		public NetworkPlayerSettings PlayerSettings;
	}
}
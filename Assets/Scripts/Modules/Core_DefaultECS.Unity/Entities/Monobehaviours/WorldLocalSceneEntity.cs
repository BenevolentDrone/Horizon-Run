using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class WorldLocalSceneEntity : ASceneEntity
	{
		[SerializeField]
		private string worldID;

		public string WorldID => worldID;
	}
}
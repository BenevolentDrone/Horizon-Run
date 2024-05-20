using UnityEngine;

namespace HereticalSolutions.Entities
{
	public class WorldLocalSceneEntity : ASceneEntity
	{
		[SerializeField]
		private string worldID;

		public string WorldID => worldID;
	}
}
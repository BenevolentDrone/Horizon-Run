using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public abstract class ASceneEntity : MonoBehaviour
	{
		[SerializeField]
		private string prototypeID;

		public List<ChildEntityDescriptor> ChildEntities;

		public string PrototypeID
		{
			get => prototypeID;
		}
	}
}
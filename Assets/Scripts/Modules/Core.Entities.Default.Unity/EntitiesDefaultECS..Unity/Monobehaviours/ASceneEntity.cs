using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.Entities
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
using HereticalSolutions.Pools;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class UnityPhysicsBodyDescriptor : MonoBehaviour
	{
		public string PrototypeID;

		public Transform Transform;

		public LayerMask LayerMask;

		public Rigidbody Rigidbody;

		public Collider[] Colliders;

		public IPoolElementFacade<GameObject> PoolElement;
	}
}
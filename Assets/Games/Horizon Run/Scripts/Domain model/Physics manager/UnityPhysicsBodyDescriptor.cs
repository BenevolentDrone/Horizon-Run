using HereticalSolutions.Pools;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	public class UnityPhysicsBodyDescriptor : MonoBehaviour
	{
		public string PrototypeID;

		public Transform Transform;

		public LayerMask LayerMask;

		public Rigidbody Rigidbody;

		public Collider[] Colliders;

		public IPoolElement<GameObject> PoolElement;
	}
}
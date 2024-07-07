using UnityEngine;

public class TEST : MonoBehaviour
{
    [SerializeField]
    private Vector3 force;

    [SerializeField]
    private Vector3 angularForce;

    [SerializeField]
    private Rigidbody rigidbody;

    void Awake()
    {
        //rigidbody.angularVelocity = angularForce;

        rigidbody.AddForce(
            force,
            ForceMode.Force);
    }
}

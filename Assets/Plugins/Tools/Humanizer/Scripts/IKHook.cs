using UnityEngine;
 
[ExecuteInEditMode]
public class IKHook : MonoBehaviour
{
    public Animator animator;

    public GameObject leftHandIKGoal;
    public GameObject rightHandIKGoal;

    public Vector3 leftHandIKGoalPosition { get; private set; }
    public Vector3 rightHandIKGoalPosition { get; private set; }

    public Quaternion leftHandIKGoalRotation { get; private set; }
    public Quaternion rightHandIKGoalRotation { get; private set; }

    public float leftHandPositionWeight = 0;
    public float rightHandPositionWeight = 0;
    
    public float leftHandRotationWeight = 0;
    public float rightHandRotationWeight = 0;

    public bool DirectControl = false;

    void OnAnimatorIK(int layerIndex)
    {
        //Debug.Log(string.Format("OnAnimatorIK, weights: {0} {1} , goals : {2} {3}", leftHandPositionWeight, rightHandPositionWeight, leftHandIKGoal.transform.parent.name, rightHandIKGoal.transform.parent.name));

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotationWeight);

        if (leftHandPositionWeight > 0.1f)
        {
            if (leftHandIKGoal != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKGoal.transform.position);

                leftHandIKGoalPosition = leftHandIKGoal.transform.position;
            }
        }
        else
            leftHandIKGoalPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand);

        if (leftHandRotationWeight > 0.1f)
        {
            if (leftHandIKGoal != null)
            {
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKGoal.transform.rotation);

                leftHandIKGoalRotation = leftHandIKGoal.transform.rotation;
            }
        }
        else
            leftHandIKGoalRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);


        if (rightHandPositionWeight > 0.1f)
        {
            if (rightHandIKGoal != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKGoal.transform.position);

                rightHandIKGoalPosition = rightHandIKGoal.transform.position;
            }
        }
        else
            rightHandIKGoalPosition = animator.GetIKPosition(AvatarIKGoal.RightHand);

        if (rightHandRotationWeight > 0.1f)
        {
            if (rightHandIKGoal != null)
            {
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKGoal.transform.rotation);

                rightHandIKGoalRotation = rightHandIKGoal.transform.rotation;
            }
        }
        else
            rightHandIKGoalRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
    }

    void Update()
    {
        if (DirectControl)
        {
            //Debug.Log("EditorIK.Update()");

            animator.Update(0);
        }
    }
}
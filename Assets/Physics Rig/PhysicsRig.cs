using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Transform playerHead, leftController, rightController;
    public ConfigurableJoint headJoint, leftHandJoint, rightHandJoint;
    public CapsuleCollider bodyCollider;

    public float bodyHeightMin = 0.5f, bodyHeightMax = 2f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);

        leftHandJoint.targetPosition = leftController.localPosition;
        leftHandJoint.targetRotation = leftController.localRotation;

        rightHandJoint.targetPosition = rightController.localPosition;
        rightHandJoint.targetRotation = rightController.localRotation;

        headJoint.targetPosition = playerHead.localPosition;
    }
}

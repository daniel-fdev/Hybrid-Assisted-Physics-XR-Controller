using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovePhysics : MonoBehaviour
{
    public float speed = 1, turnSpeed = 60, jumpHeight = 1.5f;
    private float jumpVel = 7f;
    public InputActionProperty moveInputSource, turnInputSource, jumpInputSource;
    public Rigidbody rb, leftHandRB, rightHandRB;

    public bool onlyMoveWhenGrounded = false, jumpWithHand = true;
    public float minJumpWithHandSpeed = 2, maxJumpWithHandSpeed = 7;

    public Transform dirSource, turnSource;
    public CapsuleCollider bodyCollider;
    private Vector2 inputMoveAxis;
    private float inputTurnAxis;
    private bool isGrounded;

    public LayerMask groundLayer;
    
    private Transform currentPlatform;
    private Vector3 localOffset;

    private bool isClimbing = false;

    public bool rotationCompensation = true, positionCompensation;
    float lastPlatformYRot = 0f;
    float vel1;

    // CALLED FROM CLIMB SYSTEM
    public void SetClimbing(bool climbing)
    {
        isClimbing = climbing;
        currentPlatform = null; // force clean re-attach on landing after climb
    }
    
    void Update()
    {
        if (isClimbing) return;

        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();
        if (!jumpWithHand)
        {
            if (jumpInput && isGrounded)
            {
                jumpVel = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
                rb.linearVelocity = Vector3.up * jumpVel;
            }
        }
        else
        {
            bool inputJumpPressed = jumpInputSource.action.IsPressed();
            float handSpeed = ((leftHandRB.linearVelocity - rb.linearVelocity).magnitude + (rightHandRB.linearVelocity - rb.linearVelocity).magnitude) / 2;

            if (inputJumpPressed && isGrounded && handSpeed > minJumpWithHandSpeed)
            {
                rb.linearVelocity = Vector3.up * Mathf.Clamp(handSpeed, minJumpWithHandSpeed, maxJumpWithHandSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing) return;

        RaycastHit hit;
        isGrounded = CheckIfGrounded(out hit);
        Transform hitPlatform = isGrounded ? (hit.rigidbody != null ? hit.rigidbody.transform : hit.transform) : null;

        if ((isGrounded && onlyMoveWhenGrounded) || !onlyMoveWhenGrounded)
        {
            Quaternion yaw = Quaternion.Euler(0, dirSource.eulerAngles.y, 0);
            Vector3 dir = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);
            Vector3 ownWorldMove = dir * Time.fixedDeltaTime * speed;

            float angle = turnSpeed * Time.fixedDeltaTime * inputTurnAxis;
            Quaternion ownTurn = Quaternion.AngleAxis(angle, Vector3.up);

            // standing on platform
            if (hitPlatform != null)
            {
                // inv quaternion of platform * vector3 distance = localOffset of player on the platform
                if (currentPlatform != hitPlatform)
                {
                    currentPlatform = hitPlatform;
                    localOffset = Quaternion.Inverse(hitPlatform.rotation) *
                                  new Vector3(rb.position.x - hitPlatform.position.x, 0,
                                              rb.position.z - hitPlatform.position.z);
                }
                else
                {
                    Vector3 worldOffset = hitPlatform.rotation * localOffset;
                    Vector3 worldPos = hitPlatform.position + worldOffset;
                    
                    worldPos += ownWorldMove;
                    
                    Vector3 pivot = turnSource.position;
                    pivot.y = worldPos.y;
                    Vector3 fromPivot = worldPos - pivot;
                    fromPivot = ownTurn * fromPivot;
                    worldPos = pivot + fromPivot;
                    
                    Vector3 newWorldOffset = worldPos - hitPlatform.position;
                    localOffset = Quaternion.Inverse(hitPlatform.rotation) *
                                  new Vector3(newWorldOffset.x, 0, newWorldOffset.z);
                }

                //compensate rotation when platform is turning
                float deltaYaw = Mathf.DeltaAngle(lastPlatformYRot, hitPlatform.eulerAngles.y);
                lastPlatformYRot = hitPlatform.eulerAngles.y;
                Quaternion platformTurn = Quaternion.Euler(0, deltaYaw, 0);

                //float smoothedTurnSpeed = Mathf.SmoothDamp(transform.eulerAngles.y, inputTurnAxis, ref vel1, 0.1f);
                
                //calculate final values
                Vector3 finalWorldOffset = hitPlatform.rotation * localOffset;
                Vector3 platformTrackedPos = new Vector3(
                            hitPlatform.position.x + finalWorldOffset.x,
                            rb.position.y,
                            hitPlatform.position.z + finalWorldOffset.z);
                
                Vector3 targetPos = positionCompensation ? platformTrackedPos : new Vector3(rb.position.x + ownWorldMove.x, rb.position.y, rb.position.z + ownWorldMove.z);

                rb.MoveRotation(rb.rotation * ownTurn * (rotationCompensation ? platformTurn : Quaternion.identity));
                rb.MovePosition(targetPos);
            }
            else // no moving platform
            {
                currentPlatform = null;
                
                Vector3 targetMovePos = rb.position + ownWorldMove;
                Vector3 pivot = turnSource.position;
                Vector3 newPos = ownTurn * (targetMovePos - pivot) + pivot;

                rb.MoveRotation(rb.rotation * ownTurn);
                rb.MovePosition(newPos);
            }
        }
        else // in air
        {
            currentPlatform = null;
        }
    }

    public bool CheckIfGrounded(out RaycastHit hitInfo)
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        return Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out hitInfo, rayLength, groundLayer);
    }
}
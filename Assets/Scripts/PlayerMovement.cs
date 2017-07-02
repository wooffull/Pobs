using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float walkSpeed = 1000.0f;
    public float jumpHeight = 500.0f;
    public float totalJumpTime = 1.0f;
    public float snapDistance = 3.0f;

    private Camera cam;
    private Rigidbody rigidBody;
    
    private bool canJump = false;
    private float jumpTimer = 0;
    private RaycastHit isGroundedHitInfo;
    private Vector3 displacement;
    private bool isGrounded;
    private HashSet<GameObject> collidedGroundObjects;
    
	// Use this for initialization
	void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rigidBody = GetComponent<Rigidbody>();

        collidedGroundObjects = new HashSet<GameObject>();
    }

    // FixedUpdate is called on a reliable timer, independent of frame rate
    void Update()
    {
        // Reset movement for this tick
        displacement = Vector3.zero;

        UpdateMovementAndRotation();
        UpdateJump();

        rigidBody.MovePosition(displacement + transform.position);
    }

    void OnCollisionStay(Collision c)
    {
        if (c.contacts.Length > 0)
        {
            ContactPoint contact = c.contacts[0];

            // If the collision was below the player, the player is grounded
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                collidedGroundObjects.Add(c.gameObject);
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision c)
    {
        collidedGroundObjects.Remove(c.gameObject);

        // If there are no game objects directly under the player, it's no longer grounded
        if (collidedGroundObjects.Count == 0)
        {
            isGrounded = false;
        }
    }

    private void UpdateMovementAndRotation()
    {
        // Use only the x- and z-components of the camera's forward
        Vector3 camProjectedForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Vector3 camProjectedRight = new Vector3(cam.transform.forward.z, 0, -cam.transform.forward.x).normalized;

        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        if (hAxis != 0 || vAxis != 0)
        {
            // Calculate the angle offset for the camera's forward
            float offsetAngle = Vector3.Angle(Vector3.forward, camProjectedForward);
            if (Vector3.Dot(camProjectedForward, Vector3.right) < 0)
            {
                offsetAngle *= -1;
            }

            // Calculate the movement direction for this frame, then rotate it so it's relative from the
            // camera's forward
            Vector3 movementDirection = new Vector3(hAxis, 0, vAxis).normalized;
            movementDirection = Quaternion.Euler(0, offsetAngle, 0) * movementDirection;
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }

        // Add movement from walking around
        displacement += (camProjectedForward * vAxis + camProjectedRight * hAxis) * walkSpeed * Time.deltaTime;
    }

    private void UpdateJump()
    {
        bool jumpKeyDown = Input.GetButton("Jump");
        float prevJumpTimer = jumpTimer;
        float deltaTimeSinceLastJump = Time.deltaTime;

        // If no jump key is down, or the jump has finished, the player can no longer jump
        if (!jumpKeyDown || jumpTimer >= totalJumpTime)
        {
            canJump = false;
        }

        // Otherwise, if the player can jump, jump!
        else if (canJump)
        {
            if (jumpTimer < totalJumpTime)
            {
                jumpTimer += deltaTimeSinceLastJump;

                // Do not allow the jump timer to exceed the total time allow for holding a jump
                if (jumpTimer > totalJumpTime)
                {
                    deltaTimeSinceLastJump = jumpTimer - totalJumpTime;
                    jumpTimer = totalJumpTime;
                }
            }
        }

        // If the player is jumping, prepare to move this game object up in space
        if (canJump && jumpTimer > 0)
        {
            displacement += Vector3.up * CalculateJumpIncrement(prevJumpTimer, jumpTimer);
        }

        // Reset jump variables if grounded and can jump
        if (isGrounded && !canJump)
        {
            ResetJump();
        }
    }

    /**
     * Returns the distance the player should rise from t0 seconds to t1 seconds in the jump cycle to abide by a parabolic motion
     */
    private float CalculateJumpIncrement(float t0, float t1)
    {
        float offset0 = t0 - totalJumpTime;
        float offset1 = t1 - totalJumpTime;
        return (jumpHeight / (totalJumpTime * totalJumpTime)) * (offset1 * offset1 * offset1 - offset0 * offset0 * offset0);
    }

    private void ResetJump()
    {
        jumpTimer = 0;
        canJump = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float walkSpeed = 1000.0f;
    public float turnSpeed = 20.0f;
    public float jumpSpeed = 500.0f;
    public float snapDistance = 3.0f;

    private const int MAX_JUMP_TIMER = 15;
    private const float JUMP_LERP_INTERPOLANT = 0.1f;

    private Camera cam;
    private CharacterController characterController;

    private Vector3 impulse = Vector3.zero;
    private bool canJump = false;
    private int jumpTimer = 0;

	// Use this for initialization
	void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
    }
	
	// FixedUpdate is called on a reliable timer, independent of frame rate
	void FixedUpdate()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        characterController.SimpleMove(transform.forward * vAxis * walkSpeed);
        transform.Rotate(Vector3.up * turnSpeed * hAxis, Space.World);

        SnapToRamp();
        UpdateJump();
    }

    private void UpdateJump()
    {
        bool jumpKeyDown = Input.GetButton("Jump");

        // If jump key is down and player can jump, jump!
        if (jumpKeyDown && canJump)
        {
            if (jumpTimer < MAX_JUMP_TIMER)
            {
                jumpTimer++;
                AddImpulse(Vector3.up * jumpSpeed);
            }
            else
            {
                canJump = false;
            }
        }
        else
        {
            canJump = false;
        }

        // If the impulse is big enough, apply it
        // (This is so that we're not applying very small impulses, which
        // may lead to jitters or sliding)
        if (impulse.magnitude > 0.2f)
        {
            characterController.Move(impulse);
        }

        // Reset jump variables if grounded and can jump
        if (characterController.isGrounded)
        {
            if (!canJump)
            {
                jumpTimer = 0;
                canJump = true;
            }
        }
        
        // Only lerp the impulse to zero when not grounded
        else
        {
            impulse = Vector3.Lerp(impulse, Vector3.zero, JUMP_LERP_INTERPOLANT);
        }
    }

    private void SnapToRamp()
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray testRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(testRay, out hitInfo, snapDistance))
        {
            characterController.Move(hitInfo.point - transform.position);
        }
    }

    private void AddImpulse( Vector3 impulse )
    {
        this.impulse += impulse;
    }
}

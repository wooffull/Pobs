using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float walkSpeed = 1000.0f;
    public float turnSpeed = 20.0f;

    private Camera cam;
    private CharacterController characterController;

	// Use this for initialization
	void Start () {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
    }
	
	// FixedUpdate is called on a reliable timer, independent of frame rate
	void FixedUpdate () {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        float moveSpeed = walkSpeed;
        Vector3 movement = transform.forward * vAxis * moveSpeed;

        characterController.SimpleMove(movement);
        transform.Rotate(Vector3.up * turnSpeed * hAxis, Space.World);
    }
}

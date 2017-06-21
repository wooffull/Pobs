using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToRamp : MonoBehaviour {
    public float snapDistance = 3.0f;

    private CharacterController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo = new RaycastHit();
        Ray testRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(testRay, out hitInfo, snapDistance)) {
            controller.Move(hitInfo.point - transform.position);
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonCamFollow : MonoBehaviour {
    public Vector3 offsetPosition;
    
    public float horizontalTurnSpeed = 90;
    public float verticalTurnSpeed = 90;

    public float minVerticalAngle = 20;
    public float maxVerticalAngle = 60;

    [SerializeField]
    private Transform target;

    private Quaternion qX;
    private Quaternion qY;

    void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        qX = Quaternion.identity;
        qY = Quaternion.identity;
    }
	
    // Update is called once per frame
    void LateUpdate ()
    {
        // Get the axes for the right analog stick
        float hAxis = Input.GetAxis("HorizontalCam");
        float vAxis = Input.GetAxis("VerticalCam");
        
        // Add rotation from the right analog stick
        qX *= Quaternion.AngleAxis(hAxis * horizontalTurnSpeed * Time.deltaTime, Vector3.up);
        qY *= Quaternion.AngleAxis(vAxis * verticalTurnSpeed * Time.deltaTime, Vector3.right);

        // Clamp the vertical angle between its min and max
        float verticalAngle = Vector3.Angle(Vector3.up, (qX * qY) * offsetPosition);

        if (verticalAngle < minVerticalAngle)
        {
            qY = Quaternion.AngleAxis(verticalAngle - minVerticalAngle, Vector3.right) * qY;
        }
        else if (verticalAngle > maxVerticalAngle)
        {
            qY = Quaternion.AngleAxis(verticalAngle - maxVerticalAngle, Vector3.right) * qY;
        }

        // Move camera to its new position and look at the target
        transform.position = target.position + (qX * qY) * offsetPosition;
        transform.LookAt(target);
    }
}

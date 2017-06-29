using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonCamFollow : MonoBehaviour {
    public Vector3 offsetPosition;

    [SerializeField]
    private Transform target;

    void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
    // Update is called once per frame
    void Update ()
    {
        transform.position = target.TransformPoint(offsetPosition);
        transform.LookAt(target);
    }
}

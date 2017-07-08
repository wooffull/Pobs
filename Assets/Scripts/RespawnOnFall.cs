using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    public float minY = 0;
    
    public Vector3 RespawnPosition { get; set; }

	// Update is called once per frame
	void Update () {
        if (transform.position.y < minY)
        {
            transform.position = new Vector3
            (
                RespawnPosition.x,
                RespawnPosition.y - minY,
                RespawnPosition.z
            );
        }
	}
}

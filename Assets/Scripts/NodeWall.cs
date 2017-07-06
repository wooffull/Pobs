using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeWall : MonoBehaviour {
    public GameObject NodeWallObject { get; set; }
    public RoomNode RoomNode { get; set; }

    // Update is called once per frame
    void Update () {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.tag == "Enemy")
            {
                return;
            }
        }
        
        RoomNode.Unlock();
	}

    void OnDestroy()
    {
        DestroyWall();
    }

    public void DestroyWall()
    {
        Destroy(NodeWallObject);
    }
}

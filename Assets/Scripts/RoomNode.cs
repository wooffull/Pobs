using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour {
    public NodeWall Wall { get; set; }
    public List<RoomNode> Neighbors { get; set; }

    void Awake()
    {
        Neighbors = new List<RoomNode>();
    }

    public void Unlock()
    {
        foreach (RoomNode neighbor in Neighbors)
        {
            neighbor.Wall.DestroyWall();
        }

        Wall.DestroyWall();
    }
}

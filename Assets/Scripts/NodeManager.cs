using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager instance = null;

    private Object[] nodes;

    void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }

        // If it already exists and it's not this, then destroy this (to enforce our Singleton pattern)
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // This won't be destroyed when reloading scenes
        DontDestroyOnLoad(gameObject);

        nodes = Resources.LoadAll("Nodes");
    }

    public Object GetNode()
    {
        return nodes[Random.Range(0, nodes.Length)];
    }

    public Object GetNode(int id)
    {
        return nodes[id];
    }
}

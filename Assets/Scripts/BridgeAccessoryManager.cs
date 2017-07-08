using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAccessoryManager : MonoBehaviour
{
    public static BridgeAccessoryManager instance = null;

    private Object[] bridgeAccessories;

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

        bridgeAccessories = Resources.LoadAll("BridgeAccessories");
    }

    public Object GetBridgeAccessory()
    {
        return bridgeAccessories[Random.Range(0, bridgeAccessories.Length)];
    }

    public Object GetBridgeAccessory(int id)
    {
        return bridgeAccessories[id];
    }
}

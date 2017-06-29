using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    private Object platformPrefab;

    void Start()
    {
        platformPrefab = Resources.Load("HalfSphere");
    }

    public object CreatePlatform(float x, float y, float z)
    {
        return Instantiate(
            platformPrefab,
            new Vector3(x, y, z),
            Quaternion.identity
        );
    }
}

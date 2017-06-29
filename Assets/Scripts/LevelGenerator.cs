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

    public void GenerateLevel(int horizontalNodeLength = 10, int verticalNodeLength = 10)
    {
        Map map = new Map(horizontalNodeLength, verticalNodeLength);

        for (int i = 0; i < map.HorizontalLength; i++)
        {
            for (int j = 0; j < map.VerticalLength; j++)
            {
                MapNode n = map.Nodes[i][j];

                if (n.Active)
                {
                    CreatePlatform(i * 100, 0, j * 100);
                }
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3
        (
            map.StartNode.X * 100,
            100,
            map.StartNode.Y * 100
        );
    }
}

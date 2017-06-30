using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    private Object platformPrefab;
    private Object bridgePrefab;

    void Start()
    {
        platformPrefab = Resources.Load("HalfSphere");
        bridgePrefab = Resources.Load("Bridge");
    }

    public object CreatePlatform(float x, float y, float z)
    {
        return Instantiate(
            platformPrefab,
            new Vector3(x, y, z),
            Quaternion.identity
        );
    }

    public object CreateBridge(float x, float y, float z, float angle)
    {
        return Instantiate(
            bridgePrefab,
            new Vector3(x, y, z),
            Quaternion.Euler(0, angle, 0)
        );
    }


    public void GenerateLevel(int horizontalNodeLength = 50, int verticalNodeLength = 50)
    {
        //ProceduralLevelBinaryTree levelTree = new ProceduralLevelBinaryTree(10);
        ProceduralLevelMap2D map = new ProceduralLevelMap2D(10, 10);


        for (int i = 0; i < map.HorizontalLength; i++)
        {
            for (int j = 0; j < map.VerticalLength; j++)
            {
                Map2DNode n = map.Nodes[i][j];

                CreatePlatform(i * 100, 0, j * 100);

                if (n.Down != null)
                {
                    CreateBridge(i * 100, 0, j * 100 + 50, 0);
                }

                if (n.Right != null)
                {
                    CreateBridge(i * 100 + 50, 0, j * 100, 90);
                }
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3
        (
            0,
            100,
            0
        );
    }







    /*public void GenerateLevel(int horizontalNodeLength = 10, int verticalNodeLength = 10)
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
    }*/
}

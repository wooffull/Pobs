using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    [SerializeField]
    private Object platformPrefab;

    [SerializeField]
    private Object bridgePrefab;

    public object CreatePlatform(float x, float y, float z)
    {
        return Instantiate(
            platformPrefab,
            new Vector3(x, y, z),
            Quaternion.identity
        );
    }

    public object CreateBridge(float x, float y, float z, float angle, float scale)
    {
        object ret = Instantiate(
            bridgePrefab,
            new Vector3(x, y, z),
            Quaternion.Euler(0, angle, 0)
        );
        GameObject bridge = (GameObject)ret;
        Vector3 prevScale = bridge.transform.localScale;
        bridge.transform.localScale = new Vector3
        (
            prevScale.x,
            prevScale.y,
            scale
        );
        return ret;
    }


    public void GenerateLevel(uint horizontalNodeLength = 25, uint verticalNodeLength = 25)
    {
        ProceduralLevelMap2D map = new ProceduralLevelMap2D(5, 5);

        Bounds bounds = CalculateLocalBounds((GameObject)platformPrefab);
        float nodeDistance = bounds.size.x * 1.2f;
        float bridgeOffsetY = -7;

        for (int i = 0; i < map.HorizontalLength; i++)
        {
            for (int j = 0; j < map.VerticalLength; j++)
            {
                Map2DNode n = map.Nodes[i][j];

                // Transform the indices to positions that have a hexagonal pattern
                float x = i * nodeDistance;
                float z = j * nodeDistance;
                if (i % 2 == 0) z += nodeDistance * 2 * Mathf.Floor(j / 2);
                else z += nodeDistance * 2 * Mathf.Floor((j + 1) / 2) - nodeDistance;

                if (n.Active)
                {
                    CreatePlatform(x, 0, z);

                    if (n.ConnectedWithDown)
                    {
                        CreateBridge(x, bridgeOffsetY, z + nodeDistance * 0.5f, 0, nodeDistance);
                    }

                    if (n.ConnectedWithRight)
                    {
                        float nextX = (i + 1) * nodeDistance;
                        float nextZ = j * nodeDistance;
                        if ((i + 1) % 2 == 0) nextZ += nodeDistance * 2 * Mathf.Floor(j / 2);
                        else nextZ += nodeDistance * 2 * Mathf.Floor((j + 1) / 2) - nodeDistance;

                        float bridgeX = (nextX + x) * 0.5f;
                        float bridgeZ = (nextZ + z) * 0.5f;

                        Vector3 curPos = new Vector3(x, 0, z);
                        Vector3 nextPos = new Vector3(nextX, 0, nextZ);
                        Vector3 direction = nextPos - curPos;
                        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

                        CreateBridge(bridgeX, bridgeOffsetY, bridgeZ, angle, nodeDistance * Mathf.Sqrt(2));
                    }
                }
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float startX = map.StartNode.X * nodeDistance;
        float startZ = map.StartNode.Y * nodeDistance;

        if (map.StartNode.X % 2 == 0) startZ += nodeDistance * 2 * Mathf.Floor(map.StartNode.Y / 2);
        else startZ += nodeDistance * 2 * Mathf.Floor((map.StartNode.Y + 1) / 2) - nodeDistance;

        player.transform.position = new Vector3
        (
            startX,
            100,
            startZ
        );
    }

    private Bounds CalculateLocalBounds( GameObject parent )
    {
        // First find a center for your bounds.
        Vector3 center = Vector3.zero;

        foreach (Transform child in parent.transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }
        center /= parent.transform.childCount; //center is average center of children

        //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
        Bounds bounds = new Bounds(center, Vector3.zero);

        foreach (Transform child in parent.transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }

        return bounds;
    }
}

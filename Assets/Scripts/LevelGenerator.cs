using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public float nodeSeparationMultiplier = 1.5f;

    [SerializeField]
    private Object platformPrefab;

    [SerializeField]
    private Object bridgePrefab;

    [SerializeField]
    private Object nodeWallPrefab;

    private NodeManager nodeManager;
    private BridgeAccessoryManager bridgeAccessoryManager;

    void Start()
    {
        GameObject preloadedApp = GameObject.Find("__app");
        nodeManager = preloadedApp.GetComponent<NodeManager>();
        bridgeAccessoryManager = preloadedApp.GetComponent<BridgeAccessoryManager>();
    }

    public object CreateNode(float x, float y, float z)
    {
        GameObject nodeWallObject = (GameObject)Instantiate(
            nodeWallPrefab,
            new Vector3(x, y, z),
            Quaternion.identity
        );
        GameObject node = (GameObject)Instantiate(
            nodeManager.GetNode(),
            new Vector3(x, y, z),
            Quaternion.identity
        );

        NodeWall nodeWall = node.AddComponent<NodeWall>();
        nodeWall.NodeWallObject = nodeWallObject;

        RoomNode roomNode = node.AddComponent<RoomNode>();
        roomNode.Wall = nodeWall;
        nodeWall.RoomNode = roomNode;

        return node;
    }

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

    public object CreateBridgeAccessory(object bridge)
    {
        GameObject bridgeGameObject = (GameObject)bridge;
        Bounds bridgeBounds = CalculateLocalBounds(bridgeGameObject);

        object ret = Instantiate(
            bridgeAccessoryManager.GetBridgeAccessory(),
            bridgeGameObject.transform.position,
            Quaternion.AngleAxis(90, Vector3.up) * bridgeGameObject.transform.rotation
        );

        GameObject accessoryGameObject = (GameObject)ret;
        Bounds accessoryBounds = CalculateLocalBounds(accessoryGameObject);

        // Offset the center of the accessory so it lays on top of the bridge
        accessoryGameObject.transform.position += new Vector3
        (
            0,
            (bridgeBounds.size.y + bridgeBounds.size.y) * 0.5f,
            0
        );

        return ret;
    }

    public void GenerateLevel(uint horizontalNodeLength = 25, uint verticalNodeLength = 25)
    {
        ProceduralLevelMap2D map = new ProceduralLevelMap2D(horizontalNodeLength, verticalNodeLength);
        AddMapToWorld(map);
        LinkRoomNeighbors(map);
    }

    private void AddMapToWorld(Map2D map)
    {
        Bounds bounds = CalculateLocalBounds((GameObject)platformPrefab);
        float nodeDistance = bounds.size.x * nodeSeparationMultiplier;
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
                    n.GameObject = (GameObject)CreateNode(x, 0, z);

                    if (n.ConnectedWithDown)
                    {
                        object bridge = CreateBridge(x, bridgeOffsetY, z + nodeDistance * 0.5f, 0, nodeDistance);

                        if (Random.value < 0.25f)
                        {
                            CreateBridgeAccessory(bridge);
                        }
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

                        object bridge = CreateBridge(bridgeX, bridgeOffsetY, bridgeZ, angle, nodeDistance * Mathf.Sqrt(2));
                        
                        if (Random.value < 0.25f)
                        {
                            CreateBridgeAccessory(bridge);
                        }
                    }
                }
            }
        }

        // Add player to the world after everything's been added
        float startX = map.StartNode.X * nodeDistance;
        float startZ = map.StartNode.Y * nodeDistance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (map.StartNode.X % 2 == 0) startZ += nodeDistance * 2 * Mathf.Floor(map.StartNode.Y / 2);
        else startZ += nodeDistance * 2 * Mathf.Floor((map.StartNode.Y + 1) / 2) - nodeDistance;

        player.transform.position = new Vector3
        (
            startX,
            100,
            startZ
        );

        RespawnOnFall respawnComponent = player.GetComponent<RespawnOnFall>();
        respawnComponent.RespawnPosition = new Vector3
        (
            startX,
            100,
            startZ
        );
    }

    private void LinkRoomNeighbors(Map2D map)
    {
        for (int i = 0; i < map.HorizontalLength; i++)
        {
            for (int j = 0; j < map.VerticalLength; j++)
            {
                Map2DNode n = map.Nodes[i][j];

                if (n.GameObject != null)
                {
                    RoomNode roomNode = n.GameObject.GetComponent<RoomNode>();

                    foreach (Map2DNode neighbor in n.ConnectedNodes)
                    {
                        RoomNode neighborRoomNode = neighbor.GameObject.GetComponent<RoomNode>();
                        roomNode.Neighbors.Add(neighborRoomNode);
                    }
                }
            }
        }
    }

    private Bounds CalculateLocalBounds( GameObject parent )
    {
        // First find a center for your bounds.
        Vector3 center = Vector3.zero;

        foreach (Transform child in parent.transform)
        {
            Renderer childRenderer = child.gameObject.GetComponent<Renderer>();

            if (childRenderer != null)
            {
                center += childRenderer.bounds.center;
            }
        }
        center /= parent.transform.childCount; //center is average center of children

        //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
        Bounds bounds = new Bounds(center, Vector3.zero);

        foreach (Transform child in parent.transform)
        {
            Renderer childRenderer = child.gameObject.GetComponent<Renderer>();

            if (childRenderer != null)
            {
                bounds.Encapsulate(childRenderer.bounds);
            }
        }

        return bounds;
    }
}

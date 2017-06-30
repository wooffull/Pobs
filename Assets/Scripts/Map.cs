using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Map {
    private const float DEFAULT_CONNECTION_PERCENTAGE = 0.3f;
    private const float SKIP_CONNECTION_PERCENTAGE = 0.85f;
    private const float SKIP_CONNECTION_BACKUP_PERCENTAGE = 0.5f;

    private int horizontalLength;
    private int verticalLength;

    public Map2DNode[][] Nodes { get; set; }
    public int HorizontalLength { get { return horizontalLength; } }
    public int VerticalLength { get { return verticalLength; } }

    public Map2DNode StartNode { get; set; }

    public Map(int horizontalNodes, int verticalNodes, float connectionPercentage = DEFAULT_CONNECTION_PERCENTAGE)
    {

        Assert.IsTrue(horizontalNodes > 0 && verticalNodes > 0);
        Assert.IsTrue(connectionPercentage >= 0 && connectionPercentage <= 1);

        horizontalLength = horizontalNodes;
        verticalLength = verticalNodes;

        // Construct map data from nodes
        InitializeNodes(HorizontalLength, VerticalLength);
        LinkAdjacentNodes();
        RandomizeStartNode();

        // Add necessary connections
        int connectionsNeeded = Mathf.FloorToInt(HorizontalLength * VerticalLength * connectionPercentage);

        while (connectionsNeeded > 0)
        {
            AddConnection();
            connectionsNeeded--;
        }
    }

    public void AddConnection()
    {
        bool connectionAdded = false;
        Stack<Map2DNode> nodeStack = new Stack<Map2DNode>();
        Map2DNode searchNode;
        List<Map2DNode> adjacentNodes;
        List<Map2DNode> connectedUnvistedNodes;
        List<Map2DNode> availableNodes;

        ResetNodes();
        nodeStack.Push(StartNode);

        Debug.Log("START");

        int attempts = 0;

        while (connectionAdded == false)
        {
            // TODO: Figure out why the loop doesn't stop
            if (attempts == 20) break;
            attempts++;

            // Get top node on stack
            searchNode = nodeStack.Peek();
            Assert.IsNotNull(searchNode);

            // It is now a visited node
            searchNode.Visited = true;

            // Get references to the node data around the search node
            adjacentNodes = searchNode.AdjacentNodes;
            connectedUnvistedNodes = searchNode.ConnectedUnvisistedNodes;
            availableNodes = searchNode.AvailableNodes;

            // If no connected nodes, add one to this node
            if (adjacentNodes.Count > 0 && adjacentNodes.Count == availableNodes.Count)
            {
                int nodeIndexToConnect = Random.Range(0, adjacentNodes.Count);
                Debug.Log("Default add " + nodeIndexToConnect.ToString());
                Map2DNode nodeToConnect = adjacentNodes[nodeIndexToConnect];
                searchNode.Connect(nodeToConnect);
                connectionAdded = true;
            }

            // If any connections have been made, keep searching for a place to add the connection
            else
            {
                bool canBackup = nodeStack.Count > 1;

                // If no nodes available for connection and we can pop the stack, pop this node off to look for other connections
                if (canBackup && availableNodes.Count == 0)
                {
                    Debug.Log("None available..." + availableNodes.Count.ToString());
                    nodeStack.Pop();
                }

                // Otherwise, decide between adding a connection, or continuing to look for a spot to add a connection
                else
                {
                    bool addConnection = availableNodes.Count > 0 && Random.Range(0.0f, 1.0f) > SKIP_CONNECTION_PERCENTAGE;

                    // By chance, we are adding the connection
                    if (addConnection)
                    {
                        int nodeIndexToConnect = Random.Range(0, availableNodes.Count);
                        Debug.Log("New add " + availableNodes.Count);
                        Map2DNode nodeToConnect = availableNodes[nodeIndexToConnect];
                        searchNode.Connect(nodeToConnect);
                        connectionAdded = true;
                    }

                    // Or, we are skipping the connection and looking for another place to add one
                    else
                    {
                        bool backup = 
                            nodeStack.Count > 1 &&
                            (connectedUnvistedNodes.Count == 0 ||
                            Random.Range(0.0f, 1.0f) < SKIP_CONNECTION_BACKUP_PERCENTAGE);

                        // By chance, we are going back one node in the stack
                        if (backup)
                        {
                            Debug.Log("Backup...");
                            nodeStack.Pop();
                        }

                        // Or, we are proceeding forward to an unvisited, connected node
                        else if (connectedUnvistedNodes.Count > 0)
                        {
                            Debug.Log("Proceed..." + connectedUnvistedNodes.Count.ToString() + " " + nodeStack.Count.ToString());
                            int nextNodeIndex = Random.Range(0, connectedUnvistedNodes.Count);
                            Map2DNode nextNode = connectedUnvistedNodes[nextNodeIndex];
                            nodeStack.Push(nextNode);
                        }

                        // If we've gotten stuck, move to a random adjacent node by default
                        else
                        {
                            Debug.Log("Adj...");
                            int nextNodeIndex = Random.Range(0, adjacentNodes.Count);
                            Map2DNode nextNode = adjacentNodes[nextNodeIndex];
                            nodeStack.Push(nextNode);
                        }
                    }
                }
            }
        }
    }

    private void InitializeNodes(int horizontalLength, int verticalLength)
    {
        Nodes = new Map2DNode[horizontalLength][];
        for (uint i = 0; i < horizontalLength; i++)
        {
            Nodes[i] = new Map2DNode[verticalLength];

            for (uint j = 0; j < verticalLength; j++)
            {
                Nodes[i][j] = new Map2DNode(i, j);
            }
        }
    }
    
    private void LinkAdjacentNodes()
    {
        int horizontalLength = Nodes.Length;
        int verticalLength = Nodes[0].Length;

        // Link adjacent nodes together
        for (int i = 0; i < Nodes.Length; i++)
        {
            for (int j = 0; j < Nodes[0].Length; j++)
            {
                Map2DNode node = Nodes[i][j];

                int leftIndex = i - 1;
                int rightIndex = i + 1;
                int upIndex = j - 1;
                int downIndex = j + 1;

                if (leftIndex >= 0 && leftIndex < horizontalLength)
                {
                    node.Left = Nodes[leftIndex][j];
                }
                if (rightIndex >= 0 && rightIndex < horizontalLength)
                {
                    node.Right = Nodes[rightIndex][j];
                }
                if (upIndex >= 0 && upIndex < verticalLength)
                {
                    node.Up = Nodes[i][upIndex];
                }
                if (downIndex >= 0 && downIndex < verticalLength)
                {
                    node.Down = Nodes[i][downIndex];
                }
            }
        }
    }

    private void RandomizeStartNode()
    {
        int randomX = Random.Range(0, HorizontalLength);
        int randomY = Random.Range(0, VerticalLength);

        StartNode = Nodes[randomX][randomY];
        StartNode.Active = true;
    }

    private void ResetNodes()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            for (int j = 0; j < Nodes[0].Length; j++)
            {
                Nodes[i][j].Reset();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ProceduralLevelMap2D : Map2D {
    private const int MAX_CYCLE_COUNT = 3;
    private const float DEFAULT_FILL_PERCENTAGE = 0.35294117647f;
    private const float CONTINUE_CONNECTION_SEARCH_PERCENTAGE = 0.95f;
    private const float CONNECT_CYCLE_PERCENTAGE = 0.1f;

    private List<Map2DNode> activeNodes;
    private int[][] adjacencyMatrix;
    private bool[][] cycleMatrix;
    private uint cycleCount;

    public ProceduralLevelMap2D(uint horizontalNodes, uint verticalNodes, float connectionPercentage = DEFAULT_FILL_PERCENTAGE)
        : base(horizontalNodes, verticalNodes)
    {
        uint totalNodes = horizontalNodes * verticalNodes;

        // Seed random
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int elapsedSeconds = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        // Seed == 1498861726 : 3 cycles
        // Seed == 1498863788 : 1 cycle
        // Seed == 1498863869 : 1 medium cycle
        // Seed == 1498863869 : 2 cycles (medium + small)
        Random.InitState(elapsedSeconds);
        Debug.Log("Seed: " + elapsedSeconds.ToString());

        activeNodes = new List<Map2DNode>();
        adjacencyMatrix = new int[totalNodes][];
        cycleMatrix = new bool[totalNodes][];
        cycleCount = 0;

        // Fill matrices with default values
        for (uint i = 0; i < totalNodes; i++)
        {
            adjacencyMatrix[i] = new int[totalNodes];
            cycleMatrix[i] = new bool[totalNodes];

            for (uint j = 0; j < totalNodes; j++)
            {
                // The main diagonal has different values
                if (i == j)
                {
                    adjacencyMatrix[i][j] = 0;
                    cycleMatrix[i][j] = true;
                }
                else
                {
                    adjacencyMatrix[i][j] = -1;
                    cycleMatrix[i][j] = false;
                }
            }
        }

        // Add necessary connections
        int connectionsNeeded = Mathf.FloorToInt(HorizontalLength * VerticalLength * connectionPercentage);

        while (connectionsNeeded > 0)
        {
            AddConnection();
            connectionsNeeded--;
        }
        Debug.Log(cycleCount);
    }

    public void AddConnection()
    {
        // TODO: Add MapNodeData to the nodes

        // If there isn't a start node, set it at the center
        if (StartNode == null)
        {
            StartNode = Nodes[HorizontalLength / 2][VerticalLength / 2];
            StartNode.Active = true;
            activeNodes.Add(StartNode);
            //StartNode.Data = new MapNodeData(5, 5, 5);
            return;
        }

        bool connectionAdded = false;
        Map2DNode searchNode;
        Stack<Map2DNode> nodeStack = new Stack<Map2DNode>();
        List<Map2DNode> adjacentNodes;
        List<Map2DNode> connectedUnvistedNodes;
        List<Map2DNode> availableNodes;

        ResetNodes();

        // Select a random active node
        int randomNodeIndex = Random.Range(0, activeNodes.Count);
        searchNode = activeNodes[randomNodeIndex];
        nodeStack.Push(searchNode);

        // Search for and decide on a place to make the connection
        while (connectionAdded == false)
        {
            // Get top node on stack
            searchNode = nodeStack.Peek();
            Assert.IsNotNull(searchNode);

            // It is now a visited node
            searchNode.Visited = true;

            // Get references to the node data around the search node
            adjacentNodes = searchNode.AdjacentNodes;
            connectedUnvistedNodes = searchNode.ConnectedUnvisistedNodes;
            availableNodes = searchNode.AvailableNodes;

            bool useNext =
                connectedUnvistedNodes.Count > 0 &&
                (availableNodes.Count == 0 ||
                Random.value <= CONTINUE_CONNECTION_SEARCH_PERCENTAGE);

            // By chance, move to an already-connected node to try to make the new connection off that one
            if (useNext)
            {
                int nextNodeIndex = Random.Range(0, connectedUnvistedNodes.Count);
                Map2DNode nextNode = connectedUnvistedNodes[nextNodeIndex];
                nodeStack.Push(nextNode);
                continue;
            }

            // (If possible) Add the connection to this node
            if (availableNodes.Count > 0)
            {
                int nodeToAddIndex = Random.Range(0, availableNodes.Count);
                Map2DNode nodeToAdd = availableNodes[nodeToAddIndex];
                bool createsCycle = nodeToAdd.Active;
                bool canCreate =
                    !createsCycle ||
                    (createsCycle &&
                    cycleCount < MAX_CYCLE_COUNT &&
                    Random.value <= CONNECT_CYCLE_PERCENTAGE);

                if (canCreate)
                {
                    searchNode.Connect(nodeToAdd);

                    // Give a weight to the connection
                    int weight = Random.Range(5, 10);
                    adjacencyMatrix[searchNode.Id][nodeToAdd.Id] = weight;
                    adjacencyMatrix[nodeToAdd.Id][searchNode.Id] = weight;

                    if (createsCycle)
                    {
                        // Record the cycle
                        cycleMatrix[searchNode.Id][nodeToAdd.Id] = true;
                        cycleMatrix[nodeToAdd.Id][searchNode.Id] = true;
                        cycleCount++;
                    }

                    connectionAdded = true;
                    activeNodes.Add(searchNode);
                    continue;
                }
            }

            // (If possible) Pop the stack and try from that node again
            if (nodeStack.Count > 1)
            {
                nodeStack.Pop();
                continue;
            }

            // (Dead end!) Retry the whole process...
            ResetNodes();
            nodeStack.Clear();

            // Select a random active node
            randomNodeIndex = Random.Range(0, activeNodes.Count);
            searchNode = activeNodes[randomNodeIndex];
            nodeStack.Push(searchNode);
        }
    }

    protected override void LinkNodes()
    {
        // Link nodes together in a hexagonal pattern
        for (int i = 0; i < HorizontalLength; i++)
        {
            for (int j = 0; j < VerticalLength; j++)
            {
                Map2DNode node = Nodes[i][j];

                bool useTopForVertical = (i + j) % 2 == 1;
                int leftIndex = i - 1;
                int rightIndex = i + 1;
                int verticalIndex;

                // The 2D map will be parsed into a hexagonal view later on. Each vertex of the hexagon
                // has at most 3 connections, so we will preserve that requirement in this data.
                if (useTopForVertical) verticalIndex = j - 1;
                else verticalIndex = j + 1;

                // If not out of bounds, add left and right nodes
                if (leftIndex >= 0 && leftIndex < HorizontalLength) node.Left = Nodes[leftIndex][j];
                if (rightIndex >= 0 && rightIndex < HorizontalLength) node.Right = Nodes[rightIndex][j];

                // If not out of bounds, add up or down node (depending on which one is being used for the vertical)
                if (verticalIndex >= 0 && verticalIndex < VerticalLength)
                {
                    if (useTopForVertical) node.Up = Nodes[i][verticalIndex];
                    else node.Down = Nodes[i][verticalIndex];
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Map2D
{
    private uint horizontalLength;
    private uint verticalLength;

    public Map2DNode[][] Nodes { get; set; }
    public uint HorizontalLength { get { return horizontalLength; } }
    public uint VerticalLength { get { return verticalLength; } }

    public Map2DNode StartNode { get; set; }

    public Map2D(uint horizontalNodes, uint verticalNodes)
    {
        Assert.IsTrue(horizontalNodes > 0 && verticalNodes > 0);

        horizontalLength = horizontalNodes;
        verticalLength = verticalNodes;

        // Construct map data from nodes
        InitializeNodes(HorizontalLength, VerticalLength);
        LinkNodes();
    }

    private void InitializeNodes(uint horizontalLength, uint verticalLength)
    {
        Nodes = new Map2DNode[horizontalLength][];
        uint totalAdded = 0;
        for (uint i = 0; i < horizontalLength; i++)
        {
            Nodes[i] = new Map2DNode[verticalLength];

            for (uint j = 0; j < verticalLength; j++)
            {
                Nodes[i][j] = new Map2DNode(i, j, totalAdded);
                totalAdded++;
            }
        }
    }

    protected virtual void LinkNodes()
    {
        // Link adjacent nodes together
        for (int i = 0; i < HorizontalLength; i++)
        {
            for (int j = 0; j < VerticalLength; j++)
            {
                Map2DNode node = Nodes[i][j];

                int leftIndex = i - 1;
                int rightIndex = i + 1;
                int upIndex = j - 1;
                int downIndex = j + 1;

                if (leftIndex >= 0 && leftIndex < HorizontalLength)
                {
                    node.Left = Nodes[leftIndex][j];
                }
                if (rightIndex >= 0 && rightIndex < HorizontalLength)
                {
                    node.Right = Nodes[rightIndex][j];
                }
                if (upIndex >= 0 && upIndex < VerticalLength)
                {
                    node.Up = Nodes[i][upIndex];
                }
                if (downIndex >= 0 && downIndex < VerticalLength)
                {
                    node.Down = Nodes[i][downIndex];
                }
            }
        }

        // Set the start node as the first node
        StartNode = Nodes[0][0];
    }

    protected virtual void ResetNodes()
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

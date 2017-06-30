using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelMap2D : Map2D {
   /* public static ProceduralLevelMap2D CreateFromTree(ProceduralLevelBinaryTree tree)
    {


        ProceduralLevelMap2D map = new ProceduralLevelMap2D();
    }*/

    public ProceduralLevelMap2D(uint horizontalNodes, uint verticalNodes)
        : base(horizontalNodes, verticalNodes)
    {
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

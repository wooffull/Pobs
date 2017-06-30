using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelBinaryTree : BinaryTree<MapNodeData>
{
    private int count;

    public int Count
    {
        get { return count; }
    }
    
    public ProceduralLevelBinaryTree(int nodesToAdd = 0)
    {
        count = 0;

        for (int i = 0; i < nodesToAdd; i++)
        {
            AddRandomNode();
        }
    }

    public void AddRandomNode()
    {
        int nodeWidth = Random.Range(3, 5);
        int nodeHeight = Random.Range(3, 5);
        int id = count;

        MapNodeData nodeData = new MapNodeData(nodeWidth, nodeHeight, id);
        BinaryTreeNode<MapNodeData> node = new BinaryTreeNode<MapNodeData>(nodeData);

        // If there currently is no root node, set it to the newly created node
        if (Root != null)
        {
            Root = node;
        }

        // Otherwise, add the new node to a random spot in the tree
        else
        {
            BinaryTreeNode<MapNodeData> searchNode = Root;
            bool added = false;

            while (!added)
            {
                bool traverseLeft = Random.Range(0.0f, 1.0f) < 0.5f;
                
                // 50% chance to go left
                if (traverseLeft)
                {
                    if (searchNode.Left == null)
                    {
                        searchNode.Left = node;
                        added = true;
                    }
                    else
                    {
                        searchNode = searchNode.Left;
                    }
                }

                // 50% chance to go right
                else
                {
                    if (searchNode.Right == null)
                    {
                        searchNode.Right = node;
                        added = true;
                    }
                    else
                    {
                        searchNode = searchNode.Right;
                    }
                }
            }
        }

        count++;
    }
}

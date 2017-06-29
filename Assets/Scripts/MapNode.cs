using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode {
    public int X { get; set; }
    public int Y { get; set; }
    public bool Visited { get; set; }
    public bool Active { get; set; }
    private bool ConnectedWithLeft { get; set; }
    private bool ConnectedWithRight { get; set; }
    private bool ConnectedWithUp { get; set; }
    private bool ConnectedWithDown { get; set; }
    public MapNode Left { get; set; }
    public MapNode Right { get; set; }
    public MapNode Up { get; set; }
    public MapNode Down { get; set; }
    public List<MapNode> AdjacentNodes
    {
        get
        {
            List<MapNode> adjacentNodes = new List<MapNode>();
            if (Left != null) adjacentNodes.Add(Left);
            if (Right != null) adjacentNodes.Add(Right);
            if (Up != null) adjacentNodes.Add(Up);
            if (Down != null) adjacentNodes.Add(Down);

            return adjacentNodes;
        }
    }
    public List<MapNode> AvailableNodes
    {
        get
        {
            List<MapNode> availableNodes = new List<MapNode>();
            if (Left != null && !Left.ConnectedWithRight && !Left.Visited) availableNodes.Add(Left);
            if (Right != null && !Right.ConnectedWithLeft && !Right.Visited) availableNodes.Add(Right);
            if (Up != null && !Up.ConnectedWithDown && !Up.Visited) availableNodes.Add(Up);
            if (Down != null && !Down.ConnectedWithUp && !Down.Visited) availableNodes.Add(Down);

            return availableNodes;
        }
    }
    public List<MapNode> ConnectedNodes
    {
        get
        {
            List<MapNode> connectedNodes = new List<MapNode>();
            if (Left != null && Left.ConnectedWithRight) connectedNodes.Add(Left);
            if (Right != null && Right.ConnectedWithLeft) connectedNodes.Add(Right);
            if (Up != null && Up.ConnectedWithDown) connectedNodes.Add(Up);
            if (Down != null && Down.ConnectedWithUp) connectedNodes.Add(Down);

            return connectedNodes;
        }
    }
    public List<MapNode> ConnectedUnvisistedNodes
    {
        get
        {
            List<MapNode> connectedUnvisitedNodes = new List<MapNode>();
            if (Left != null && Left.ConnectedWithRight && !Left.Visited) connectedUnvisitedNodes.Add(Left);
            if (Right != null && Right.ConnectedWithLeft && !Right.Visited) connectedUnvisitedNodes.Add(Right);
            if (Up != null && Up.ConnectedWithDown && !Up.Visited) connectedUnvisitedNodes.Add(Up);
            if (Down != null && Down.ConnectedWithUp && !Down.Visited) connectedUnvisitedNodes.Add(Down);

            return connectedUnvisitedNodes;
        }
    }

    public MapNode(int x, int y)
    {
        X = x;
        Y = y;
        Active = false;
        ConnectedWithLeft = false;
        ConnectedWithRight = false;
        ConnectedWithUp = false;
        ConnectedWithDown = false;

        Reset();
    }

    public void Reset()
    {
        Visited = false;
    }

    public void Connect(MapNode node)
    {
        if (node == Left)
        {
            node.ConnectedWithRight = true;
            ConnectedWithLeft = true;
        }
        
        if (node == Right)
        {
            node.ConnectedWithLeft = true;
            ConnectedWithRight = true;
        }

        if (node == Up)
        {
            node.ConnectedWithDown = true;
            ConnectedWithUp = true;
        }

        if (node == Down)
        {
            node.ConnectedWithUp = true;
            ConnectedWithDown = true;
        }

        Active = true;
        node.Active = true;
    }
}

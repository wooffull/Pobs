﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2DNode {
    public uint X { get; set; }
    public uint Y { get; set; }
    public bool Visited { get; set; }
    public bool Active { get; set; }
    private bool ConnectedWithLeft { get; set; }
    private bool ConnectedWithRight { get; set; }
    private bool ConnectedWithUp { get; set; }
    private bool ConnectedWithDown { get; set; }
    public Map2DNode Left { get; set; }
    public Map2DNode Right { get; set; }
    public Map2DNode Up { get; set; }
    public Map2DNode Down { get; set; }
    public List<Map2DNode> AdjacentNodes
    {
        get
        {
            List<Map2DNode> adjacentNodes = new List<Map2DNode>();
            if (Left != null) adjacentNodes.Add(Left);
            if (Right != null) adjacentNodes.Add(Right);
            if (Up != null) adjacentNodes.Add(Up);
            if (Down != null) adjacentNodes.Add(Down);

            return adjacentNodes;
        }
    }
    public List<Map2DNode> AvailableNodes
    {
        get
        {
            List<Map2DNode> availableNodes = new List<Map2DNode>();
            if (Left != null && !Left.ConnectedWithRight && !Left.Visited) availableNodes.Add(Left);
            if (Right != null && !Right.ConnectedWithLeft && !Right.Visited) availableNodes.Add(Right);
            if (Up != null && !Up.ConnectedWithDown && !Up.Visited) availableNodes.Add(Up);
            if (Down != null && !Down.ConnectedWithUp && !Down.Visited) availableNodes.Add(Down);

            return availableNodes;
        }
    }
    public List<Map2DNode> ConnectedNodes
    {
        get
        {
            List<Map2DNode> connectedNodes = new List<Map2DNode>();
            if (Left != null && Left.ConnectedWithRight) connectedNodes.Add(Left);
            if (Right != null && Right.ConnectedWithLeft) connectedNodes.Add(Right);
            if (Up != null && Up.ConnectedWithDown) connectedNodes.Add(Up);
            if (Down != null && Down.ConnectedWithUp) connectedNodes.Add(Down);

            return connectedNodes;
        }
    }
    public List<Map2DNode> ConnectedUnvisistedNodes
    {
        get
        {
            List<Map2DNode> connectedUnvisitedNodes = new List<Map2DNode>();
            if (Left != null && Left.ConnectedWithRight && !Left.Visited) connectedUnvisitedNodes.Add(Left);
            if (Right != null && Right.ConnectedWithLeft && !Right.Visited) connectedUnvisitedNodes.Add(Right);
            if (Up != null && Up.ConnectedWithDown && !Up.Visited) connectedUnvisitedNodes.Add(Up);
            if (Down != null && Down.ConnectedWithUp && !Down.Visited) connectedUnvisitedNodes.Add(Down);

            return connectedUnvisitedNodes;
        }
    }

    public Map2DNode(uint x, uint y)
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

    public void Connect(Map2DNode node)
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
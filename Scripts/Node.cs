using System;
using System.Collections.Generic;
using UnityEngine;

// This class represents a structure of type node and will be use to build upon the 'Tree' like structure.
// The structure will have a single root node and each consecutive node will be a child of a parent node.
// The children will be stored in the child node list allowing only 2 children per node
public abstract class Node
{
    private List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList; }

    public bool Visted { get; set; }

    // Creating each corner of the mesh as the room will be a rectangular space
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }

    public Node Parent { get; set; }

    // Index used for tree structure
    public int TreeLayerIndex { get; set; }

    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);

    }

    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}
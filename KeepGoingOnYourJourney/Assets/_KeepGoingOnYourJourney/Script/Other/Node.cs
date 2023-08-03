using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private bool walkable;
    private bool isDead;
    private Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public List<Node> neighbours;
    public enum SpecialLand
    {
        IncScore = 0,
        MinusSore = 1
    }
    public SpecialLand specialLand; 

    public Node(bool _walkable, bool _isDead, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        isDead = _isDead;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    public bool Iswalkable()
    {
        return walkable;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void SetWalkable(bool walkable)
    {
        this.walkable = walkable;
    }
    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    public Vector3 GetWorldPos()
    {
        return worldPosition;
    }

    public void SetNeighbours(List<Node> _neighbours)
    {
        neighbours = _neighbours;
    }
}

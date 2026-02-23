using UnityEngine;

public class Node
{
    public Vector3Int position;
    public int gCost;
    public int hCost;
    public Node parent; 

    public int fCost => gCost + hCost;
    public Node(Vector3Int pos)
    {
        position = pos;
    }
}

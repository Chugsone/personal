using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    public Tilemap groundTilemap;
    private Vector3 gizmoTarget;
    private bool[,] walkableGrid;
    private Vector3Int gridOrigin;
    private int gridWidth; 
    private int gridHeight; 


    private BoundsInt debugBounds;

    void Start()
    {
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Vector3Int start = groundTilemap.WorldToCell(startWorld);
        Vector3Int target = groundTilemap.WorldToCell(targetWorld);  

        if (!IsWalkable(start) || !IsWalkable(target))
        {
            return null;
        }

        gizmoTarget = target;
        List<Node> openList = new List<Node>();
        HashSet<Vector3Int> closedSet = new ();

        Node startNode = new Node(start);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Debug.Log("test");
            Node currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.position);

            if (currentNode.position == target)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector3Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (!IsWalkable(neighborPos) || closedSet.Contains(neighborPos))
                {
                    continue;
                }

                Debug.Log(" WALKABLE");
                int newCost = currentNode.gCost + ((neighborPos.x != currentNode.position.x && neighborPos.y != currentNode.position.y) ? 14 : 10);
                Node neighborNode = openList.Find(n => n.position == neighborPos);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborPos)
                    {
                        gCost = newCost,
                        hCost = GetDistance(neighborPos, target),
                        parent = currentNode
                    };
                    openList.Add(neighborNode);
                }
                else if (newCost < neighborNode.gCost)
                {
                    neighborNode.gCost = newCost;
                    neighborNode.parent = currentNode;
                }
            }
        }

        
        return null;
    }


    public void BuildGrid(BoundsInt bounds) //Could likely do part of this incide the wfcgen
    {
        debugBounds = bounds;

        gridWidth = bounds.size.x;
        gridHeight = bounds.size.y;
        gridOrigin = bounds.min;

        walkableGrid = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                walkableGrid[x, y] = true;
            }
        }
    }

    private bool IsWalkable(Vector3Int gridPos)
    {
        int x = gridPos.x - gridOrigin.x;
        int y = gridPos.y - gridOrigin.y;

        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
        {
            return false;
        }
        return walkableGrid[x, y];
    }


    public void SetBlocked(Vector3 worldPos, bool blocked)
    {
        Vector3Int cell = groundTilemap.WorldToCell(worldPos);
        int x = cell.x - gridOrigin.x;
        int y = cell.y - gridOrigin.y;

        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            walkableGrid[x, y] = !blocked;
        }

    }

    public void SetBlocked(Collider2D col, bool blocked)
    {
        Bounds bounds = col.bounds;

        Vector3Int min = groundTilemap.WorldToCell(bounds.min);
        Vector3Int max = groundTilemap.WorldToCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cell = new (x, y, 0);

                int gx = cell.x - gridOrigin.x;
                int gy = cell.y - gridOrigin.y;

                if (gx >= 0 && gy >= 0 && gx < gridWidth && gy < gridHeight)
                {
                    walkableGrid[gx, gy] = !blocked;
                }

            } 
        }

    }


    private int GetDistance(Vector3Int a, Vector3Int b)
    {

        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        int diagonal = Mathf.Min(dx, dy);
        int straight = Mathf.Abs(dx - dy);


        return 14 * diagonal + 10 * straight;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new();

        Vector3Int up = pos + Vector3Int.up;
        Vector3Int down = pos + Vector3Int.down;
        Vector3Int left = pos + Vector3Int.left;
        Vector3Int right = pos + Vector3Int.right;

        if (IsWalkable(up))
        {
            neighbors.Add(up);
        }
        if (IsWalkable(down))
        {
            neighbors.Add(down);
        }
        if (IsWalkable(left))
        {
            neighbors.Add(left);
        }
        if (IsWalkable(right))
        {
            neighbors.Add(right);
        }

        Vector3Int upRight = pos + new Vector3Int(1, 1, 0);
        if (IsWalkable(upRight) && IsWalkable(up) && IsWalkable(right))
        {
            neighbors.Add(upRight);
        }
        Vector3Int upLeft = pos + new Vector3Int(-1, 1, 0);
        if (IsWalkable(upLeft) && IsWalkable(up) && IsWalkable(left))
        {
            neighbors.Add(upLeft);
        }
        Vector3Int downRight = pos + new Vector3Int(1, -1, 0);
        if (IsWalkable(downRight) && IsWalkable(down) && IsWalkable(right))
        {
            neighbors.Add(downRight);
        }
        Vector3Int downLeft = pos + new Vector3Int(-1, -1, 0);
        if (IsWalkable(downLeft) && IsWalkable(down) && IsWalkable(left))
        {
            neighbors.Add(downLeft);
        }

        Debug.Log(neighbors.Count);
        return neighbors;
        
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new ();
        Node current = endNode;
        while(current != startNode)
        {
            path.Add(groundTilemap.GetCellCenterWorld(current.position));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gizmoTarget, Vector3.one);

        if (debugBounds.size == Vector3Int.zero)
        {
            return;
        }

        Vector3 minWorld = groundTilemap.CellToWorld(debugBounds.min);
        Vector3 maxWorld = groundTilemap.CellToWorld(debugBounds.max);


        Vector3 center = (minWorld + maxWorld) / 2f;
        Vector3 size = maxWorld - minWorld + groundTilemap.cellSize;

        Gizmos.DrawWireCube(center, size);



        
    }
}

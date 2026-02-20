using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class WFCGen : MonoBehaviour
{
    public enum ExitDirections
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
    }
    [Tooltip("The max amount of rooms to spawn")][SerializeField] private int maxRooms = 5;
    [Tooltip("The starting room.")][SerializeField] private RoomData startRoom;
    [Tooltip("The list of all rooms")][SerializeField] private RoomData[] allRooms;
    [Tooltip("Starting Offset")][SerializeField] private Vector3Int startOffset = new Vector3Int(0, 0, 0);

    private bool geenrated;
    private GameObject gatePrefab;
    private Tilemap tilemap;
    private Dictionary<Vector2Int, RoomData> grid = new();
    private List<Vector2Int> frontier = new();

    private void Awake()
    {
        gatePrefab = Resources.Load<GameObject>("Prefabs/Gate");
    }

    void Start()
    {
        tilemap = GameObject.FindWithTag("Ground").GetComponent<Tilemap>();
        Generate(startOffset);


    }

    public void Generate(Vector3Int pos)
    {
        if (geenrated)
        {
            return;
        }
        geenrated = true;
        startOffset = pos;
        grid.Clear();
        frontier.Clear();

        Vector2Int startPos = Vector2Int.zero; //Maybe add offset here
        PlaceRoom(startRoom, startPos);
        frontier.Add(startPos);

        int roomsPlaced = 1;

        while (roomsPlaced < maxRooms && frontier.Count > 0)
        {
            int index = Random.Range(0, frontier.Count);
            Vector2Int currentPos = frontier[index];
            RoomData currentRoom = grid[currentPos];

            foreach (ExitDirections direction in System.Enum.GetValues(typeof(ExitDirections)))
            {
                Vector2Int newPos = currentPos + DirectionToVector(direction);

                if (grid.ContainsKey(newPos))
                {
                    continue;
                }

                List<RoomData> candidates = GetCompatibleRooms(currentRoom, direction);
                if (candidates.Count == 0)
                {
                    continue;
                }
                RoomData chosenRoom = ChooseWeighted(candidates);
                PlaceRoom(chosenRoom, newPos);
                frontier.Add(newPos);
                roomsPlaced++;
            }
            frontier.RemoveAt(index);
        }
    }

    private void PlaceRoom(RoomData room, Vector2Int gridPos)
    {
        

        grid[gridPos] = room;
        Debug.Log(gridPos);
        Vector3Int worldPos = startOffset + new Vector3Int(gridPos.x * (room.size.x), gridPos.y * (room.size.y), 0);
        for (int i = 0; i < room.tiles.Count; i++)
        {
            Vector3Int pos = room.positions[i] + worldPos;

            tilemap.SetTile(pos, room.tiles[i]);
        }
    }

  
      

    private bool Compatible(RoomData a, RoomData b, ExitDirections directions)
    {
        return directions switch
        {
            ExitDirections.Left => a.leftExit && b.rightExit,
            ExitDirections.Right => a.rightExit && b.leftExit,
            ExitDirections.Up => a.upExit && b.downExit,
            ExitDirections.Down => a.downExit && b.upExit,
            _ => false
        };
    }

    private List<RoomData> GetCompatibleRooms(RoomData placed, ExitDirections dir)
    {
        List<RoomData> result = new();
        foreach (RoomData room in allRooms)
        {
            if (Compatible(placed, room, dir))
            {
                result.Add(room);
            }
        }
        return result;
    }

    private RoomData ChooseWeighted(List<RoomData> rooms)
    {
        int totalWeight = 0;
        foreach (RoomData room in rooms)
        {
            totalWeight += Mathf.Max(1, room.Weight);
        }

        int roll = Random.Range(0, totalWeight);
        int running = 0;
        foreach (RoomData room in rooms)
        {
            running += Mathf.Max(1, room.Weight);
            if (roll < running)
            {
                return room;
            }
        }
        return rooms[0];
    }

    private Vector2Int DirectionToVector(ExitDirections directions)
    {
        return directions switch
        {
            ExitDirections.Left => new Vector2Int(-1, 0),
            ExitDirections.Right => new Vector2Int(1, 0),
            ExitDirections.Up => new Vector2Int(0, 1),
            ExitDirections.Down => new Vector2Int(0, -1),
            _ => Vector2Int.zero
        };
    }
}

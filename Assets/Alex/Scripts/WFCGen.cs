using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

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
    [Tooltip("The amount of rooms till the boss room spawns")] [SerializeField] private int roomsTillBoss = 50;


    private bool geenrated;
    private GameObject gatePrefab;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    private Dictionary<Vector2Int, RoomData> grid = new();
    private HashSet<Vector2Int> spawnedRooms = new();
    private List<Vector2Int> frontier = new();
    private RoomData bossHall;
    private GameObject holeGO;
    

    public GameObject latestRoom;
    public int seed;
    public bool useSeed;

    public static WFCGen Instance {get; private set; }

    private void Awake()
    {
        Instance = this;
        gatePrefab = Resources.Load<GameObject>("Prefabs/Gate");
        bossHall = Resources.Load<RoomData>("Rooms/BossHallway");
        holeGO = Resources.Load<GameObject>("Prefabs/Hole");

    }

    void Start()
    {
        Generate(startOffset, !useSeed ? null : seed);
    }

    public void Generate(Vector3Int pos, int? seed)
    {
        if (geenrated)
        {
            return;
        }

        int randomSeed = new System.Random().Next(0, 1000);
        if (!seed.HasValue)
            this.seed = randomSeed;
        
        System.Random r = new(seed ?? randomSeed);

        geenrated = true;
        startOffset = pos;
        grid.Clear();
        frontier.Clear();

        Vector2Int startPos = Vector2Int.zero; 
        grid[startPos] = startRoom;
        spawnedRooms.Add(startPos);
        PlaceRoom(startRoom, startPos);
        frontier.Add(startPos);

        int roomsPlaced = 1;

        while (roomsPlaced < maxRooms && frontier.Count > 0)
        {
            int index = r.Next(0, frontier.Count);
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
                RoomData chosenRoom = ChooseWeighted(candidates, newPos, seed);
                grid[newPos] = chosenRoom;
                //PlaceRoom(chosenRoom, newPos);
                frontier.Add(newPos);
                roomsPlaced++;
            }
            frontier.RemoveAt(index);
        }
    }

    public void PlaceRoom(Vector2Int gridPos)
    {
       PlaceRoom(grid[gridPos], gridPos); 
    }


    private void PlaceRoom(RoomData room, Vector2Int gridPos)
    {


        Vector3Int worldPos = startOffset + new Vector3Int(gridPos.x * (room.size.x), gridPos.y * (room.size.y), 0);
        
        GameObject roomGO = new GameObject($"Room_({gridPos.x}X, {gridPos.y}Y)");
        roomGO.transform.position = worldPos;

        GameObject enemiesGO = new GameObject("Room GO");
        enemiesGO.transform.parent = roomGO.transform;
        Pathfinder pathfinder = enemiesGO.AddComponent<Pathfinder>();
        pathfinder.groundTilemap = groundTilemap;
        BoundsInt roomBounds = new BoundsInt(new Vector3Int(worldPos.x - room.size.x / 2, worldPos.y - room.size.y / 2, 0), new Vector3Int(room.size.x, room.size.y, 1));
        //pathfinder.BuildGrid(roomBounds);
        latestRoom = roomGO;

        if (roomsTillBoss == 0)
        {
            room = bossHall;   
            GameObject hole = Instantiate(holeGO, (Vector3) worldPos - new Vector3(0f, 5f, 0f), Quaternion.identity);
        }
        roomsTillBoss--;
        CreateGates(room, worldPos, gridPos);

        //Debug.Log("World: " + worldPos + " -> Cell: " + groundTilemap.WorldToCell(worldPos));

       

        foreach (var tileData in room.tiles)
        {
            Vector3Int pos = tileData.position + worldPos;
            switch (tileData.layer)
            {
                case TileLayer.Background:
                    backgroundTilemap.SetTile(pos, tileData.tile);
                    break;
                case TileLayer.Ground:
                    groundTilemap.SetTile(pos, tileData.tile);
                    pathfinder.SetBlocked(groundTilemap.GetCellCenterWorld(pos), true);
                    break;
                case TileLayer.Decoration:
                    decorationTilemap.SetTile(pos, tileData.tile);
                    break;
            }
        }
        
    }


    private void CreateGates(RoomData room, Vector3 worldPos, Vector2Int gridPos)
    {
        if (room == null)
        {
            return;
        }


        if (room.leftExit)
        {
            GameObject gate = Instantiate(gatePrefab, worldPos + new Vector3(-21f, -5f, 0f), Quaternion.identity, latestRoom.transform.GetChild(0));
            gate.GetComponent<Gate>().roomPos = gridPos;
            gate.GetComponent<Gate>().ExitDirection = ExitDirections.Left;

        }

        if (room.rightExit)
        {
            GameObject gate = Instantiate(gatePrefab, worldPos + new Vector3(21f, -5f, 0f), Quaternion.identity, latestRoom.transform.GetChild(0));
            gate.GetComponent<Gate>().roomPos = gridPos;
            gate.GetComponent<Gate>().ExitDirection = ExitDirections.Right;
        }

        if (room.upExit)
        {
            GameObject gate = Instantiate(gatePrefab, worldPos + new Vector3(0f, 10f, 0f), Quaternion.identity, latestRoom.transform.GetChild(0));
            gate.GetComponent<Gate>().roomPos = gridPos;
            gate.GetComponent<Gate>().ExitDirection = ExitDirections.Up;
            gate.transform.localScale = new Vector3(4f, 1f, 1f);
        }

        if (room.downExit)
        {
            GameObject gate = Instantiate(gatePrefab, worldPos + new Vector3(0f, -10f, 0f), Quaternion.identity, latestRoom.transform.GetChild(0));
            gate.GetComponent<Gate>().roomPos = gridPos;
            gate.GetComponent<Gate>().ExitDirection = ExitDirections.Down;
            gate.transform.localScale = new Vector3(4f, 1f, 1f);



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

    private RoomData ChooseWeighted(List<RoomData> rooms, Vector2Int pos, int? seed)
    {
        string calcSeed = (seed ?? this.seed) + "" + (pos.x + 100) + "" + (pos.y + 100);
        System.Random r = new(int.Parse(calcSeed));

        int totalWeight = 0;
        foreach (RoomData room in rooms)
        {
            totalWeight += Mathf.Max(1, room.Weight);
        }

        int roll = r.Next(0, totalWeight);
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

    public bool InGrid(Vector2Int neighborGridPos)
    {
       if (spawnedRooms.Contains(neighborGridPos))
        {
            return true;
        }
        spawnedRooms.Add(neighborGridPos); //May want to change this but should work here
        return false;
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

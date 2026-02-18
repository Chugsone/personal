using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomMaker : MonoBehaviour
{
    public Tilemap sourceTilemap;
    public RoomData roomData;

    public void CaptureRoom()
    {
        if (roomData == null)
        {
            return;
            //Make it create a new room scriptable object
        }
        roomData.positions.Clear();
        roomData.tiles.Clear();

        BoundsInt bounds = sourceTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(pos);
            if (tile == null)
            {
                continue;
            }

            roomData.positions.Add(pos);
            roomData.tiles.Add(tile);


        }

        roomData.size = new Vector2Int(bounds.size.x, bounds.size.y);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(roomData);
#endif

    }



    public void LoadRoom()
    {
        sourceTilemap.ClearAllTiles();
        for (int i = 0; i < roomData.tiles.Count; i++)
        {
            Vector3Int pos = roomData.positions[i];

            sourceTilemap.SetTile(pos, roomData.tiles[i]);
        }
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomMaker : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    public Tilemap groundTilemap;
    public Tilemap decorationTilemap;

    public RoomData roomData;

    public void CaptureRoom()
    {
        if (roomData == null)
        {
            return;
            //Make it create a new room scriptable object
        }
        roomData.tiles.Clear();

        CaptureFromTilemap(backgroundTilemap, TileLayer.Background);
        CaptureFromTilemap(groundTilemap, TileLayer.Ground);
        CaptureFromTilemap(decorationTilemap, TileLayer.Decoration);

        

        BoundsInt bounds = groundTilemap.cellBounds;
        roomData.size = new Vector2Int(bounds.size.x, bounds.size.y);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(roomData);
#endif

    }

    private void CaptureFromTilemap(Tilemap tilemap, TileLayer layer)
    {
        if (tilemap == null)
        {
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null)
            {
                continue;
            }

            TilePlacement placement = new TilePlacement {tile = tile, position = pos, layer = layer};
            roomData.tiles.Add(placement);
        }
    }



    public void LoadRoom()
    {
        backgroundTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();
        
        foreach(var tileData in roomData.tiles)
        {
            switch (tileData.layer)
            {
                case TileLayer.Background:
                    backgroundTilemap.SetTile(tileData.position, tileData.tile);
                    break;
                case TileLayer.Ground:
                    groundTilemap.SetTile(tileData.position, tileData.tile);
                    break;
                case TileLayer.Decoration:
                    decorationTilemap.SetTile(tileData.position, tileData.tile);
                    break;
            }
        }
    }
}

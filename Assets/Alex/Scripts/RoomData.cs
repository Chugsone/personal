using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Level/Room Data")]
public class RoomData : ScriptableObject
{

    public Vector2Int size;
    public int Weight;
    public List<TilePlacement> tiles = new();

    public bool leftExit;
    public bool rightExit;
    public bool upExit;
    public bool downExit;


}


[System.Serializable]
public struct TilePlacement
{
    public TileBase tile;
    public Vector3Int position;
    public TileLayer layer;
}

public enum TileLayer
{
    Background,
    Ground,
    Decoration
}

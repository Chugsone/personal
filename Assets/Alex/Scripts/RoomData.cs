using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Level/Room Data")]
public class RoomData : ScriptableObject
{



    public Vector2Int size;
    public List<Vector3Int> positions = new();
    public List<TileBase> tiles = new();
    public int Weight;

    public bool leftExit;
    public bool rightExit;
    public bool upExit;
    public bool downExit;


}

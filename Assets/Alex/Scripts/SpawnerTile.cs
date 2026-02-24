using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Spawner Tile", menuName = "2D/Tiles/Spawner Tile")]
public class SpawnerTile : Tile
{
    [Tooltip("The type of enemy to spawn")] [SerializeField] GameObject enemyPrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!Application.isPlaying)
        {
            return false;
        }

        Tilemap tm = tilemap.GetComponent<Tilemap>();
        tm.SetTile(position, null);
        Vector3 worldPos = tm.GetCellCenterWorld(position);
        Instantiate (enemyPrefab, worldPos, Quaternion.identity, WFCGen.Instance.latestRoom.transform.GetChild(0));
        return true;
        
    }
}

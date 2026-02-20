using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Spawner Tile", menuName = "2D/Tiles/Spawner Tile")]
public class SpawnerTile : Tile
{
    [SerializeField] GameObject enemyPrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (Application.isPlaying)
        {
            Debug.Log("Tile spawned!");
            Instantiate(enemyPrefab, position, Quaternion.identity);
            return true;
        }

        return false;
    }
}

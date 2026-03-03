using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Spawner Tile", menuName = "2D/Tiles/Spawner Tile")]
public class SpawnerTile : Tile
{
    [Tooltip("The type of enemy to spawn")] [SerializeField] GameObject enemyPrefab;
    private static Tilemap decoration; 
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!Application.isPlaying)
        {
            return false;
        }

        if (decoration == null)
        {
            decoration = tilemap.GetComponent<Tilemap>();
        }

        //decoration.SetTile(position, null);
        Vector3 worldPos = decoration.GetCellCenterWorld(position);
        Instantiate (enemyPrefab, worldPos, Quaternion.identity, WFCGen.Instance.latestRoom.transform.GetChild(0));
        return true;
        
    }
}

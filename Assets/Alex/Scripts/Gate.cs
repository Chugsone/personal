using UnityEngine;

public class Gate : MonoBehaviour
{
   
    [HideInInspector] public WFCGen.ExitDirections ExitDirection;
    [HideInInspector] public Vector2Int roomPos;
    private static Gate latestGate;

    private void Start()
    {
        if (latestGate == null)
        {
            latestGate = this;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }

        Vector2Int neighborPos = roomPos + DirectionToVector(ExitDirection);

        Debug.Log($"Neighbor Room Pos: {neighborPos}");

        if (WFCGen.Instance.InGrid(neighborPos)) //Checks if Neighbor room has been generated.
        {
            Debug.Log("test1");
            if (latestGate.roomPos != roomPos) //This means that the player entered a different room
            {
            Debug.Log("test2");

            }
        }
        else //Neighbor room has not been generated yet
        {
            Debug.Log("test3");

        }

        latestGate = this;
        
    }

    private Vector2Int DirectionToVector(WFCGen.ExitDirections directions)
    {
        return directions switch
        {
            WFCGen.ExitDirections.Left => new Vector2Int(-1, 0),
            WFCGen.ExitDirections.Right => new Vector2Int(1, 0),
            WFCGen.ExitDirections.Up => new Vector2Int(0, 1),
            WFCGen.ExitDirections.Down => new Vector2Int(0, -1),
            _ => Vector2Int.zero
        };
    }


}

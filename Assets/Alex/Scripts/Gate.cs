using UnityEngine;

public class Gate : MonoBehaviour
{
    public enum ExitDirections
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
    }

    public ExitDirections exitDirection;
    private Vector2Int roomPos;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }

        roomPos += DirectionToVector(exitDirection);

        
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

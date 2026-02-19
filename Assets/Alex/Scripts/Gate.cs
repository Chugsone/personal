using UnityEngine;

public class Gate : MonoBehaviour
{
    public RoomData neighbor;
    public WFCGen parent;
    public Vector3Int pos;


    public void OpenGate()
    {
        Destroy(gameObject);
        parent.PlaceObjects(neighbor, pos);
    }


}

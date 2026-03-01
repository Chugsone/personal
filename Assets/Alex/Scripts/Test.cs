using UnityEngine;

public class Test : MonoBehaviour
{
    public void Test2()
    {
        gameObject.transform.GetComponentInParent<SlotMachine>().Spinning();
    }

    public void Test1()
    {
        Destroy(gameObject);
    }
}

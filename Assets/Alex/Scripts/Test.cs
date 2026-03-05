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

    public void Test3()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Instantiate(Resources.Load<GameObject>("Prefabs/BabySplit"), transform.position, Quaternion.identity, transform.parent);
        Instantiate(Resources.Load<GameObject>("Prefabs/BabySplit"), (Vector2) transform.position + new Vector2(0.5f, 0f), Quaternion.identity, transform.parent);
        Destroy(gameObject);
    }
}

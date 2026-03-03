using UnityEngine;

public class KingCan : MonoBehaviour
{

    private GameObject player;
    private Vector3 originalPosition = new(); 

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        originalPosition = transform.position;

    }

    void Update()
    {
        Vector3 eyeOffset = ((player.transform.position - originalPosition).normalized / 4f);
        eyeOffset.x = Mathf.Clamp(eyeOffset.x, -.18f, .18f);
        eyeOffset.y = Mathf.Clamp(eyeOffset.y, -.1f, .1f);

        transform.position = originalPosition + eyeOffset; // .18 .1
    }
}

using UnityEngine;

public class AudioSourceTest : MonoBehaviour
{
    public AudioSource test;
    public AudioClip clip1, clip2, clip3;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            test.PlayOneShot(clip1);
            Debug.Log("boom");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            test.PlayOneShot(clip2);
            Debug.Log("boom");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AudioSource.PlayClipAtPoint(clip3, Vector2.zero);
            Debug.Log("boom");
        }
    }
}

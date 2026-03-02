using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Hole : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private Sprite bean;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ShrinkPlayer(col.gameObject));
        }
    }

    IEnumerator ShrinkPlayer(GameObject player)
    {
        PlayerInput pI = player.GetComponent<PlayerInput>();
        pI.enabled = false;
        float index = 0;
        while (player.transform.localScale.magnitude > 0.1f)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position, speed);
            player.transform.localScale = player.transform.localScale * .95f;
            yield return new WaitForSeconds(Mathf.Clamp01(speed - speed * index));
            index += 0.05f;
        }
        pI.enabled = true;
        player.transform.localScale = Vector3.one;
        player.transform.position = Vector3.zero;
        DontDestroyOnLoad(player);
        SceneManager.LoadScene("Boss");
        yield break;
    }
    
}

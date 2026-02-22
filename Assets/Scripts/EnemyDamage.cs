using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage;
    private Stats playerHealth;
    public PlayerMovement playerMovement;

    void Start()
    {
        if (GameObject.FindWithTag("Player").TryGetComponent<Stats>(out Stats stats))
        {
            playerHealth = stats;
        }
        else
        {
            Debug.LogWarning("Player does not have stats script");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerMovement.KBCounter = playerMovement.KBTotalTime;
            if (collision.transform.position.x <= transform.position.x)
            {
                playerMovement.knockFromRight = true;
            }
            if (collision.transform.position.x > transform.position.x)
            {
                playerMovement.knockFromRight = false;
            }
            playerHealth.Health -= damage;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage;
    private Stats playerHealth;
    public Stats stats;
  
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
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.Health -= damage;
            //adds a trigger to animator component called "hit"
            Debug.Log("Hit player");
        }
    }
}

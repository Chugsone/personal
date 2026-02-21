using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public int maxHealth = 100; // Maximum health of the enemy\
    private int health; // Current health of the enemy


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth; // Initialize health to maximum at the start
    }

    public void TakeDamage(int damage)
    {
        health -= damage; // Reduce health by the damage amount
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy GameObject if health drops to zero or below
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    [Header("Particles")]
    [SerializeField] ParticleSystem healParticles;
    [SerializeField] ParticleSystem damageParticles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    public void Damae(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (damageParticles != null)
        {
            damageParticles.Play();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (healParticles != null)
        {
            healParticles.Play();
        }
    }
}

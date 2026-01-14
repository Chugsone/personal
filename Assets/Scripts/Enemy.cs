using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("Damage")]
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float attackCooldown = 1f; // seconds between damage ticks while touching player

    private Transform playerTransform;
    private Rigidbody2D rb;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
            playerTransform = playerGo.transform;
        else
            Debug.LogWarning("Enemy: no GameObject with tag 'Player' found in scene.");
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position);
        float distance = direction.magnitude;
        if (distance > stoppingDistance)
        {
            direction.Normalize();
            // use MovePosition for stable Rigidbody2D motion
            Vector2 newPos = rb.position + direction * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // If using trigger colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    // In case colliders are not triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(contactDamage);
                    lastAttackTime = Time.time;
                }
                else
                {
                    Debug.LogWarning("Enemy: collided with GameObject tagged 'Player' but no Player component found.");
                }
            }
        }
    }
}

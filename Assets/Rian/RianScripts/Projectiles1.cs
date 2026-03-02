using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectiles1 : MonoBehaviour
{
    private Collider2D col;
    private Rigidbody2D rb;

    public float knockback;
    public float speed;
    public float lifetime;
    public float knockbackTime;
    [HideInInspector] public int Damage = 1;
    [HideInInspector] public int pierceCount = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            pierceCount = 0;
        }
        else if (collision.gameObject.CompareTag("Destructible"))
        {
            pierceCount--;
            if (collision.gameObject.TryGetComponent<Destructible>(out Destructible des))
            {
                des.Destruct();
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            pierceCount--;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            pierceCount--;
            if (collision.gameObject.TryGetComponent<Stats>(out Stats enemyStats))
            {
                enemyStats.Health -= Damage;
                Debug.Log("HIT");
            }
            else
            {
                Debug.LogError("Enemy has no stats script");
            }
        }


        if (pierceCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}

   


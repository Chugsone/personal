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

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.health += 2;
            knockback -= playerMovement.health;
            Knockback(collision);
            Debug.Log($"Knockback: {knockback} --- Health: {playerMovement.health}");
        }
    }

    void Knockback(Collider2D collision)
    {
        

        Vector3 point = collision.ClosestPoint(transform.position);
        Vector2 difference = (transform.position - point).normalized;

        Vector2 launchangle = difference * knockback;



        collision.attachedRigidbody.linearVelocity = launchangle;
        

        if (collision.TryGetComponent(out PlayerMovement player))
        {
            player.StartCoroutine(adjustWeight(player));
        }
    }

    IEnumerator adjustWeight(PlayerMovement player)
    {
        player.weight = 0;

        for (float i = 0; i < knockbackTime; i+=Time.deltaTime)
        {
            player.weight = i / knockbackTime;
            yield return new WaitForEndOfFrame ();
        }

        player.weight = 1;
       
    }
}


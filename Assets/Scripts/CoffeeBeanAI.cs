using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class CoffeeBeanAI : MonoBehaviour 
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxOffset = 2f;
    [SerializeField] private float attackCooldown = 2f;

    private float attackTimer;
    private Vector3 startPosition;
    private Vector3 direction;
    private Rigidbody2D rb2d;
    private static Transform player;
    private List<Vector3> path;
    private int pathIndex;
    private bool timerActive = false;
    private bool playerFound = false;
    private float repathTimer;
    private static Stats playerStats;
    private Stats stats;

    [Tooltip("How often the enemey looks for a new path")][SerializeField] private float repathDelay = 0.5f;

    void Start()
    {
        stats = GetComponent<Stats>();
        startPosition = transform.position;
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();
            if (player.TryGetComponent<Stats>(out Stats ps))
            {
                playerStats = ps;
            }
            else
            {
                Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
            }
        }
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        pathfinder = GetComponentInParent<Pathfinder>();
        if (pathfinder == null)
        {
            Debug.LogError("Enemy Parent doesnt have a pathfinder script");
        }
    }

    void FixedUpdate()
    {
        HandlePath();
        rb2d.linearVelocity = speed * direction;

        Push();

    }

    void OnDisable()
    {
        transform.position = startPosition;
    }

    private void Push()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var col in nearby)
        {
            if (col.CompareTag("Enemy") && col.gameObject != gameObject)
            {
                Vector2 offset = transform.position - col.transform.position;
                rb2d.linearVelocity += offset.normalized * 0.5f;
            }
        }
    }


    private void HandlePath()
    {
        repathTimer -= Time.fixedDeltaTime;
        Debug.Log($"Path: {path}");
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer > 0f)
            {
                rb2d.linearVelocity = Vector3.zero;
                direction = Vector3.zero; 
            return;
            }

        if (Vector3.Distance(transform.position, player.position) < 3f)
        {
            direction = (player.position - transform.position).normalized;
            return;
        }


            if (path == null)
            {
                if (repathTimer <= 0f)
                {
                    NewPath();
                    repathTimer = repathDelay;
                }
                rb2d.linearVelocity = Vector3.zero;
                return;
            }

            if (pathIndex >= path.Count || Vector3.Distance(player.position, path[^1]) > maxOffset)
            {
                rb2d.linearVelocity = Vector3.zero;
                NewPath();
                return;
            }

            direction = path[pathIndex] - transform.position;
            direction = direction.normalized;

            if (Vector3.Distance(transform.position, path[pathIndex]) < 0.1f)
            {
                pathIndex++;
            }

        

    }

    private void Attack()
    {
        attackTimer = attackCooldown;
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(ReturnSprite());
        playerStats.Health -= stats.Damage;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Attack();
        }
    }


    IEnumerator ReturnSprite()
    {
        yield return new WaitForSeconds(attackCooldown / 3f);
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    private void NewPath()
    {
        path = pathfinder.FindPath(transform.position, player.position);
        Debug.Log($"New Path: {path}");
        pathIndex = 0;
        rb2d.linearVelocity = Vector3.zero;
    }

    private void Update()
    {

    }

}




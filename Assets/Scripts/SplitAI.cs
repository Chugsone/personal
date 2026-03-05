using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class SplitAI : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxOffset = 2f;
    [SerializeField] private float attackCooldown = 2f;

    private float originalSpeed;
    private float attackTimer;
    private Vector3 startPosition;
    private Vector3 direction;
    private Rigidbody2D rb2d;
    private static Transform player;
    private List<Vector3> path;
    private int pathIndex;
    private float repathTimer;
    private static Stats playerStats;
    private Stats stats;
    private SpriteRenderer childSR;
    private bool attacking;
    private float attackDuration = 0.5f;
    private float attackTimer2;
    private bool hitPlayer;
    private Vector3 lastSafePos;
    private GameObject deathAnim;
    private float deaggro = 10f;
    private bool buffer = false;

    [Tooltip("How often the enemey looks for a new path")][SerializeField] private float repathDelay = 0.5f;

    void Start()
    {
        deathAnim = Resources.Load<GameObject>("Prefabs/SplitPeaDeath");
        attackTimer2 = attackDuration;
        originalSpeed = speed;
        childSR = GetComponentInChildren<SpriteRenderer>();
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
        lastSafePos = transform.position;

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
        attackTimer -= Time.fixedDeltaTime;

        if (attacking)
        {
            if (!buffer)
            {
                return;
            }
            attackTimer2 -= Time.fixedDeltaTime;
            if (attackTimer2 <= 0f)
            {
                attacking = false;
                hitPlayer = false;

                speed = originalSpeed;
                attackTimer2 = attackDuration;
                GetComponent<CircleCollider2D>().isTrigger = false;
                direction = Vector3.zero;
                rb2d.linearVelocity = Vector3.zero;
                buffer = false;

            }
            return;
            
        }

        direction = Vector3.zero;
        rb2d.linearVelocity = Vector3.zero;

        

        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance > deaggro)
        {
            direction = Vector3.zero;
            rb2d.linearVelocity = Vector3.zero;
            return;
        }

        if (distance < 3f && attackTimer < 0f)
        {
            rb2d.linearVelocity = Vector3.zero;
            direction = Vector3.zero;
            Attack();
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

    private void  Attack()
    {

        attacking = true;
        direction = (player.position - transform.position).normalized;
        attackTimer = attackCooldown;
        speed = originalSpeed * 4f;
        GetComponent<CircleCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(ReturnSprite());
        StartCoroutine(Wait());

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        buffer = true;
    }


    IEnumerator ReturnSprite()
    {
        yield return new WaitForSeconds(attackCooldown / 3f);
        GetComponent<SpriteRenderer>().color = Color.white;


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !hitPlayer)
        {
            hitPlayer = true;
            playerStats.Health -= stats.Damage;
        }
        else if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Destructible"))
        {
            transform.position = lastSafePos;
            GetComponent<CircleCollider2D>().isTrigger = false;
            attacking = false;
            hitPlayer = false;

            speed = originalSpeed;
            attackTimer2 = attackDuration;
            direction = Vector3.zero;
            rb2d.linearVelocity = Vector3.zero;
            buffer = false;

        }
    }

    private void NewPath()
    {
        path = pathfinder.FindPath(transform.position, player.position);
        Debug.Log($"New Path: {path}");
        pathIndex = 0;
        rb2d.linearVelocity = Vector3.zero;
        
    }

    void OnDestroy()
    {
        if (stats.Health == 0)
        {
            Instantiate(deathAnim, lastSafePos, Quaternion.identity, transform.parent); 
            
        }
    }

}




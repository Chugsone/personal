using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BabyAI : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxOffset = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private LayerMask playerLayer;


    [HideInInspector] public bool HasShield
    {
        get { return hasSheild; }
        set { hasSheild = value; }
    }

    private float attackTimer;
    private Vector3 startPosition;
    private Vector3 direction;
    private Rigidbody2D rb2d;
    private static Transform player;
    private List<Vector3> path;
    private int pathIndex;
    private float repathTimer;
    private bool hasSheild = true;
    private static Stats playerStats;
    private SpriteRenderer childSR;
    private Stats stats;
    private Transform childGO;
    private float deaggro = 10f;

    [Tooltip("How often the enemey looks for a new path")][SerializeField] private float repathDelay = 0.5f;

    void Start()
    {
        childGO = transform.GetChild(0);
        childSR = childGO.GetComponent<SpriteRenderer>();
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
        childGO.Rotate(0, 0, speed * Time.fixedDeltaTime, Space.Self);

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
        direction = Vector3.zero;
        rb2d.linearVelocity = Vector3.zero;

        if (attackTimer > 0f)
        {
            direction = Vector3.zero;
            rb2d.linearVelocity = Vector3.zero;
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > deaggro)
        {
            direction = Vector3.zero;
            rb2d.linearVelocity = Vector3.zero;
            return;
        }
        if (distance < attackRange)
        {

            Attack();
            direction = Vector3.zero;
            rb2d.linearVelocity = Vector3.zero;
            return;
        }

        if (distance < 3f)
        {
            rb2d.linearVelocity = Vector3.zero;
            direction = player.position - transform.position;
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
        Collider2D nearby = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        attackTimer = attackCooldown;
        childSR.color = Color.red;
        StartCoroutine(ReturnColor());
        if (nearby != null)
        {
            playerStats.Health -= stats.Damage;
        }

    }

    IEnumerator ReturnColor()
    {
        yield return new WaitForSeconds(attackCooldown / 3f);
        childSR.color = Color.white;

    }

    private void NewPath()
    {
        path = pathfinder.FindPath(transform.position, player.position);
        Debug.Log($"New Path: {path}");
        pathIndex = 0;
        rb2d.linearVelocity = Vector3.zero;
    }

   

}



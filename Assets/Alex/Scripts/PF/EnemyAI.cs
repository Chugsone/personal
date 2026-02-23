using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxOffset = 2f;


    private Rigidbody2D rb2d;
    private static Transform player;
    private List<Vector3> path;
    private int pathIndex;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();
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
        if (path == null || pathIndex >= path.Count)
        {
            Debug.Log($"Path: {path}");

            NewPath();
        }

        
        if (path != null)
        {
            if (Vector3.Distance(player.position, path[^1]) > maxOffset)
            {
                NewPath();
            }
        }

        if (path != null && pathIndex < path.Count)
        {
            
            Debug.Log("test32");
            Vector2 direction = path[pathIndex] - transform.position;
            direction = direction.normalized;
            rb2d.linearVelocity = speed * direction;

            if (Vector3.Distance(transform.position, path[pathIndex]) < 0.1f)
            {
                pathIndex++;
            }

        }

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach(var col in nearby)
        {
            if (col.CompareTag("Enemy") && col.gameObject != gameObject)
            {
                Vector2 offset = transform.position - col.transform.position;
                rb2d.linearVelocity += offset.normalized * 0.5f;
            }
        }
    }

    private void NewPath()
    {
        path = pathfinder.FindPath(transform.position, player.position);
        pathIndex = 0;
        rb2d.linearVelocity = Vector3.zero;
    }
    
}


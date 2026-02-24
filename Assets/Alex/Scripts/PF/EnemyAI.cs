using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxOffset = 2f;

    private Vector3 direction;
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
        HandlePath();
         rb2d.linearVelocity = speed * direction;

        Push();

    }

    private void Push()
    {
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

    private void HandlePath()
    {
        Debug.Log($"Path: {path}");
        if (path == null || pathIndex >= path.Count)
        {
            NewPath();
            return;
        }


         if (Vector3.Distance(player.position, path[^1]) > maxOffset)
         {
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

    private void NewPath()
    {
        path = pathfinder.FindPath(transform.position, player.position);
        pathIndex = 0;
        rb2d.linearVelocity = Vector3.zero;
    }
    
}


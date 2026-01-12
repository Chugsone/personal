using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{

    private CircleCollider2D coll;

    public LayerMask enemyLayer;

    public float damage = 10f;

    public GameObject chainLightningEffect;

    public GameObject beenStruck;

    public int amountToChain = 3;

    private GameObject startObject;
    public GameObject endObject;
    private Animator ani;

    public ParticleSystem parti;

    private int singleSpawns;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (amountToChain <= 0)
        {
            Destroy(gameObject);
        }

        coll = GetComponent<CircleCollider2D>();
        ani = GetComponent<Animator>();
        parti = GetComponent<ParticleSystem>();

        startObject = gameObject;

        singleSpawns = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject target = FindNearestEnemy();
            if (target != null)
            {
                StartChainAt(target);
            }
        }
    }

    private GameObject FindNearestEnemy()
    {
        if (coll == null) return null;

        float radius = coll.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        GameObject nearest = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // skip already struck enemies (keeps existing behavior)
            if (hit.GetComponentInChildren<EnemyStruck>()) continue;

            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = hit.gameObject;
            }
        }

        return nearest;
    }

    private void StartChainAt(GameObject target)
    {
        if (target == null) return;

        // mirror the logic from OnTriggerEnter2D
        endObject = target;

        amountToChain -= 1;
        Instantiate(chainLightningEffect, target.transform.position, Quaternion.identity);

        Instantiate(beenStruck, target.transform);

        var health = target.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
        }

        if (ani != null) ani.StopPlayback();

        if (coll != null) coll.enabled = false;

        singleSpawns--;

        if (parti != null)
        {
            parti.Play();

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = startObject.transform.position;
            parti.Emit(emitParams, 1);

            emitParams.position = endObject.transform.position;
            parti.Emit(emitParams, 1);
        }

        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)) && !collision.GetComponentInChildren<EnemyStruck>())
        {
            if (singleSpawns <= 0)
            {

                endObject = collision.gameObject;

                amountToChain -= 1;
                Instantiate(chainLightningEffect, collision.gameObject.transform.position, Quaternion.identity);

                Instantiate(beenStruck, collision.gameObject.transform);

                collision.gameObject.GetComponent<EnemyHealth>().TakeDamage((int)damage);

                ani.StopPlayback();

                coll.enabled = false;

                singleSpawns --;

                parti.Play();

                var emitParams = new ParticleSystem.EmitParams();  
                emitParams.position = startObject.transform.position;

                parti.Emit(emitParams, 1);

                emitParams.position = endObject.transform.position;

                parti.Emit(emitParams, 1);

                Destroy(gameObject, 1f);
            }
        }
    }
}

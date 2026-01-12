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

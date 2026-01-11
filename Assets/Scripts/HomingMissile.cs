using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 2.5f;
    public float rotateSpeed = 200f;

    // Explosion / AOE settings
    public float explosionRadius = 2f;
    public int explosionDamage = 50;
    public GameObject explosionPrefab; // optional VFX prefab to spawn on explode
    public bool affectOnlyTaggedEnemies = true; // keep tag behavior by default

    private Transform target;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var enemyObj = GameObject.FindGameObjectWithTag("Enemy");
        if (enemyObj != null)
            target = enemyObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        var dir = (target.position - transform.position).normalized;
        float rotateAmount = Vector3.Cross(dir, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy � exploding");
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // get all colliders in AOE
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            // if configured to only affect tag-filtered enemies, skip non-enemies
            if (affectOnlyTaggedEnemies && !hit.CompareTag("Enemy"))
                continue;

            hit.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);

            // Optional: apply knockback if the hit object has a Rigidbody2D
            var otherRb = hit.attachedRigidbody;
            if (otherRb != null)
            {
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                float knockForce = 100f;
                otherRb.AddForce(knockDir * knockForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}

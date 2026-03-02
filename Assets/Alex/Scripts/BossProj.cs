using UnityEngine;

public class BossProj : MonoBehaviour
{

    [Tooltip("The speed at which the projectile travels")] [SerializeField] private float _projectileSpeed = 5f;
    [Tooltip("How long in seconds for the projectile to despawn")] [SerializeField] private float _despawnTime = 5f;
    [Tooltip("How much damage the projectile does")] [SerializeField] private int _damage = 25;
    [Tooltip("The tag of the target type")] [SerializeField] private Tag.Tags _targetType;

    private Rigidbody2D _projRB2D;
    private Vector2 direction;
    [HideInInspector] public bool IgnoreGround;
    [HideInInspector] public int Damage
    {
        get {return _damage;}
        set {_damage = value;}
    }

    [HideInInspector] public float ProjectileSpeed
    {
        get {return _projectileSpeed;}
        set {_projectileSpeed = value;}
    }

    void Start()
    {
        Destroy(gameObject, _despawnTime);
        _projRB2D = GetComponent<Rigidbody2D>();
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        direction = new(Mathf.Cos(angle), Mathf.Sin(angle)); 
    }

    void Update()
    {
        _projRB2D.linearVelocity = direction.normalized * _projectileSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.EnumTagToString(_targetType)))
        {
            Debug.Log("test");
        }
        else if (collision.gameObject.CompareTag("Ground") && !IgnoreGround)
        {
            Destroy(gameObject);
        }
        
    }
}

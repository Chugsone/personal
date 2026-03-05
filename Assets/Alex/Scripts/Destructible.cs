using UnityEngine;

public class Destructible : MonoBehaviour
{
    private ParticleSystem breakParticles;
    private bool destroyed;
    private Stats _playerStats;
    private Collider2D col; 
    private Pathfinder pf;
    //SetBlocked(Collider2D col, bool blocked)
    [Tooltip("How much exp you gain from destroying this destructible")] [SerializeField] private int _expGained = 10;

    void Awake()
    {
        if (GameObject.FindWithTag("Player").TryGetComponent<Stats>(out Stats stats))
        {
            _playerStats = stats;
        }

        if (TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            breakParticles = ps;
        }
        if (TryGetComponent<Collider2D>(out Collider2D collider))
        {
            col = collider;
        }
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent<Pathfinder>(out Pathfinder pathFinder))
        {
            pf = pathFinder;
            pf.SetBlocked(col, true);
        }
        else
        {
            Debug.LogWarning("Destructible script broken ):");
        }
    }

    public void Destruct()
    {
        if (destroyed)
        {
            return;
        }

        breakParticles.Play();
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        destroyed = true;
        _playerStats.Experience += _expGained;
        pf.SetBlocked(col, false);

    }

    void FixedUpdate()
    {
        if (destroyed)
        {
            if (!breakParticles.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}

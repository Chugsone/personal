using UnityEngine;

public class Destructible : MonoBehaviour
{
    private ParticleSystem breakParticles;
    private bool destroyed;
    private Stats _playerStats;
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
    }

    public void Destruct()
    {
        breakParticles.Play();
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        destroyed = true;
    }

    void FixedUpdate()
    {
        if (destroyed)
        {
            if (!breakParticles.IsAlive())
            {
                _playerStats.Experience += _expGained;
                Destroy(gameObject);
            }
        }
    }
}

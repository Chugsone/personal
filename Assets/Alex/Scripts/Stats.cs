using UnityEngine;

public class Stats : MonoBehaviour
{
    private static Stats _playerStats;


    [Header("Put name here :wilted_rose:")]
    [Tooltip("How much health the object has")][SerializeField] private int _maxHealth = 100;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    private int _health; 
    [HideInInspector]
    public int Health
    {
        get { return _health; }
        set
        {
            

            if (value < _health) //Removing Health
            {
                if (healParticles != null)
                {
                    damageParticles.Play();
                }
                _health = Mathf.Clamp(Mathf.RoundToInt((float)_health - (((float) _health - (float) value) * (1 - _defense))), 0, _maxHealth);
            }
            else //Adding Health
            {
                if (healParticles != null)
                {
                    healParticles.Play();
                }
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }

            if (_health <= 0)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    if (_playerStats == null)
                    {
                        Debug.LogWarning("Player Stats script not found.");
                    }
                    _playerStats.Experience += _exp;
                }
                else if (gameObject.CompareTag("Player"))
                {
                    
                }
            }
        }
    }
    
    [Tooltip("The percentage of damage mitigation 0 being none 1 being 100% mitigation")] [Range(0f, 1f)] [SerializeField] private float _defense;
    [HideInInspector] public float Defense
    {
        get { return _defense; }
        set { _defense = Mathf.Clamp(value, 0f, 1f); }
    }



    [Tooltip("How much health the object has")][SerializeField] private int _damage = 100; 
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }


    [Header("Exp stuff")]
    private int _exp;
    [HideInInspector] public int Experience
    {
        get { return _exp; }
        set
        {
            _exp = value; //Update EXP Bar
            if (_exp > _expReq)
            {
                LevelUp();
                
            }
        }
    }

    [Tooltip("How much exp is required to level up")] [SerializeField] private int _expReq = 100;
    [HideInInspector] public int ExperienceRequirement 
    {
        get { return _exp; }
        set
        {
            _expReq = value;
        }
    }

    [Tooltip("The amount increase of exp required to level up (2 being double the exp)")] [SerializeField] private float _expMultiplier = 1.5f;

    private int _level;
    [HideInInspector] public int Level
    {
        get { return _level; }
        set
        {
            Level = value; //Update EXP Bar

        }
    }

        
    [Header("Particles")]
    [SerializeField] ParticleSystem healParticles;
    [SerializeField] ParticleSystem damageParticles;

    private void LevelUp()
    {
        _exp -= _expReq;
        _level++;                
        _expReq = Mathf.RoundToInt(_expReq * _expMultiplier);

        _maxHealth = Mathf.RoundToInt(_maxHealth*1.1f);
        _health += _maxHealth / 10;
    }

    void Start()
    {
        _health = _maxHealth;
        if (gameObject.CompareTag("Player"))
        {
            _playerStats = this;
        }
    }

}
